using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Entities;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.ISolutionService))]
    public class SolutionService : ServiceAPI.ISolutionService
    {

        private Entities.IUnitOfWork uow;
        private ServiceAPI.IMessageService messageService;
        private Entities.Solution solution;
        private bool initialized = false;

        private const string NO_SOLUTION_MESSAGE = "Unable to add components to solution. Add /solution:[solutionname] as parameter to the command tool to get components attached to a specific solution.";

        [ImportingConstructor]
        public SolutionService(Entities.IUnitOfWork uow, ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        public Entities.Solution Get(string unieuqName)
        {
            return (from s in uow.Solutions.GetQuery()
                    where s.UniqueName == unieuqName
                    select s).SingleOrDefault();
        }

        public void AddMissingPluginPackage(Entities.PluginPackage package)
        {
            this.Initialize();
            if (this.solution != null)
            {
                this.AddSolutionComponent(package.PluginPackageId.Value, 10113, false);
            } 
        }

        public void AddMissingPluginAssembly(PluginAssembly assm)
        {
            this.Initialize();
            if (solution != null)
            {
                this.AddSolutionComponent(assm.PluginAssemblyId.Value, 91, false);
            }
        }
        public void AddMissingPluginTypes(PluginType[] pluginTypes)
        {
            this.Initialize();
            if (solution != null)
            {
                foreach (var plugin in pluginTypes)
                {
                    this.AddSolutionComponent(plugin.PluginTypeId.Value, 90, false);
                }
            }
        }

        public void AddMissingPluginSteps(SdkMessageProcessingStep[] steps)
        {
            this.Initialize();
            if (solution != null)
            {
                foreach (var step in steps)
                {
                    this.AddSolutionComponent(step.SdkMessageProcessingStepId.Value, 92, true);
                }
            }
        }

        private void AddSolutionComponent(Guid id, int componentType, bool addRequiredComponents)
        {
            var component = (from sc in uow.SolutionComponents.GetQuery()
                             where sc.SolutionId.Id == this.solution.SolutionId
                                && sc.ObjectId.Value == id
                                && sc.ComponentType.Value == componentType
                             select sc).SingleOrDefault();

            if (component == null)
            {
                var req = new Microsoft.Crm.Sdk.Messages.AddSolutionComponentRequest
                {
                    AddRequiredComponents = addRequiredComponents,
                    ComponentId = id,
                    ComponentType = componentType,
                    SolutionUniqueName = this.solution.UniqueName
                };
                uow.Execute(req);
                Console.WriteLine($"Added { id } of type { componentType } to solution: { this.solution.UniqueName }");
            }
        }

        private void Initialize()
        {
            if (!initialized)
            {
                var args = System.Environment.GetCommandLineArgs();

                var solutionName = (from a in args where a.ToLower().StartsWith("/solution:") select a).SingleOrDefault()?.Substring(10);

                if (solutionName == null && Kipon.Xrm.Tools.Models.Config.Instance != null)
                {
                    solutionName = Kipon.Xrm.Tools.Models.Config.Instance.Solution;
                }

                if (solutionName == null)
                {
                    Console.WriteLine(NO_SOLUTION_MESSAGE);
                    initialized = true;
                    return;
                }

                this.solution = (from s in uow.Solutions.GetQuery()
                                 where s.UniqueName == solutionName ||
                                   s.FriendlyName == solutionName
                                 select s).SingleOrDefault();

                if (this.solution == null)
                {
                    Console.WriteLine($"Solution with name {solutionName} was not found. Components will not be attached to a solution.");
                }

                initialized = true;

            }
        }
    }
}
