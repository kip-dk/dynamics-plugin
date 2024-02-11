using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Entities;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IPluginTypeService))]
    public class PluginTypeService : ServiceAPI.IPluginTypeService
    {
        private Entities.IUnitOfWork uow;
        private ServiceAPI.IMessageService messageService;

        [ImportingConstructor]
        public PluginTypeService(Entities.IUnitOfWork uow, ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        #region plugins
        public PluginType[] GetPluginTypes(Guid pluginAssemblyId)
        {
            return (from pt in uow.PluginTypes.GetQuery()
                    where pt.PluginAssemblyId.Id == pluginAssemblyId
                      && pt.IsWorkflowActivity != true
                    select pt).ToArray();
        }

        public void JoinAndCleanupPlugins(Entities.PluginType[] currents, Models.Plugin[] tobee)
        {
            foreach (var current in currents)
            {
                var inTobee = (from t in tobee
                               where t.Type.FullName == current.Name
                               select t).SingleOrDefault();

                if (inTobee != null)
                {
                    inTobee.CurrentCrmInstance = current;
                    continue;
                }
                this.Delete(current);
            }
            this.uow.ClearContext();
        }

        public void CreateAnJoinMissingPlugins(Guid pluginassemblyId, Models.Plugin[] tobees)
        {
            foreach (var tobee in tobees)
            {
                if (tobee.CurrentCrmInstance == null)
                {
                    var next = new Entities.PluginType
                    {
                        PluginTypeId = Guid.NewGuid(),
                        PluginAssemblyId = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginAssembly.EntityLogicalName, pluginassemblyId),
                        FriendlyName = tobee.Type.FullName,
                        Name = tobee.Type.FullName,
                        TypeName = tobee.Type.FullName
                    };
                    uow.Create(next);
                    tobee.CurrentCrmInstance = next;
                }
            }
        }
        #endregion

        #region workflows
        public PluginType[] GetWorkflowTypes(Guid pluginAssemblyId)
        {
            return (from pt in uow.PluginTypes.GetQuery()
                    where pt.PluginAssemblyId.Id == pluginAssemblyId
                      && pt.IsWorkflowActivity == true
                    select pt).ToArray();
        }

        public void JoinAndCleanupWorkflows(Entities.PluginType[] currents, Models.Workflow[] tobee)
        {
            foreach (var current in currents)
            {
                var inTobee = (from t in tobee
                               where t.Type.FullName == current.Name
                               select t).SingleOrDefault();

                if (inTobee != null)
                {
                    inTobee.CurrentCrmInstance = current;
                    continue;
                }
                uow.Delete(current);
            }
            this.uow.ClearContext();
        }

        public void CreateAnJoinMissingWorkflows(Guid pluginassemblyId, Models.Workflow[] tobees)
        {
            foreach (var tobee in tobees)
            {
                if (tobee.CurrentCrmInstance == null)
                {
                    var next = new Entities.PluginType
                    {
                        PluginTypeId = Guid.NewGuid(),
                        PluginAssemblyId = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginAssembly.EntityLogicalName, pluginassemblyId),
                        FriendlyName = tobee.Type.FullName,
                        Name = tobee.Type.FullName,
                        TypeName = tobee.Type.FullName,
                        WorkflowActivityGroupName = tobee.SandboxCustomActivityInfo.CustomActivityInfo.GroupName
                    };
                    uow.Create(next);
                    tobee.CurrentCrmInstance = next;
                }
            }

        }
        #endregion

        private void Delete(Entities.PluginType pluginType)
        {
            var steps = (from s in uow.SdkMessageProcessingSteps.GetQuery()
                         where s.EventHandler.Id == pluginType.PluginTypeId
                         select s).ToArray();
            foreach (var step in steps)
            {
                var images = (from i in uow.SdkMessageProcessingStepImages.GetQuery()
                              where i.SdkMessageProcessingStepId.Id == step.SdkMessageProcessingStepId
                              select i).ToArray();
                foreach (var img in images)
                {
                    uow.Delete(img);
                }
                uow.Delete(step);
            }
            uow.Delete(pluginType);

            messageService.Inform($"Removed plugin: {pluginType.Name}");
        }
    }
}
