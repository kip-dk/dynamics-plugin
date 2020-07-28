using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Entities;
using Kipon.Xrm.Tools.Models;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IPluginDeploymentService))]
    public class PluginDeploymentService : ServiceAPI.IPluginDeploymentService
    {
        private readonly ServiceAPI.IMessageService messageService;
        private readonly Entities.IUnitOfWork uow;
        private Kipon.Tools.Xrm.Reflection.Types types;
        private Kipon.Tools.Xrm.Reflection.PluginMethod.Cache pluginMethodCache;
        private int[] stages = new int[] { 10, 20, 40 };

        private string[] messages = new string[] {
            "Create",
            "Update",
            "Delete",
            "Associate",
            "Disassociate",
            "SetState",
            "SetStateDynamicEntity",
            "RetrieveMultiple",
            "Retrieve",
            "AddMember",
            "AddListMembers",
            "RemoveMember",
            "Merge"
        };

        private string[] entityLogicalNames;

        [ImportingConstructor]
        public PluginDeploymentService(ServiceAPI.IMessageService messageService, Entities.IUnitOfWork uow)
        {
            this.messageService = messageService;
            this.uow = uow;
        }

        public Plugin[] ForAssembly(Assembly assembly)
        {
            var customActions = (from sm in uow.SdkMessages.GetQuery()
                                 join wf in uow.Workflows.GetQuery() on sm.SdkMessageId equals wf.SdkMessageId.Id
                                 select sm.Name).Distinct().ToArray();
            var allActions = new List<string>(messages);
            allActions.AddRange(customActions);
            messages = allActions.ToArray();

            this.types = Kipon.Tools.Xrm.Reflection.Types.Instance;
            types.SetAssembly(assembly);
            this.pluginMethodCache = new Kipon.Tools.Xrm.Reflection.PluginMethod.Cache(assembly);

            var plugins = assembly.GetTypes().Where(r => r.BaseType == types.BasePlugin).ToArray();

            this.messageService.Inform($"Kipon.Xrm.Tools, plugin deployment found {plugins.Length} plugins in {assembly.FullName}.");

            List<Plugin> result = new List<Plugin>();

            this.entityLogicalNames = assembly.GetTypes().
                Where(r => r.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                .Select(r => ((Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(r)).LogicalName)
                .Distinct()
                .ToArray();

            this.messageService.Inform($"Found {this.entityLogicalNames.Length} entities.");

            foreach (var pluginType in plugins)
            {
                var typeName = pluginType.FullName;
                var next = new Models.Plugin(pluginType);
                result.Add(next);
                foreach (var stage in this.stages)
                {
                    foreach (var message in this.messages)
                    {
                        string[] handleEntities = this.entityLogicalNames;

                        if (Kipon.Tools.Xrm.Reflection.Types.MESSAGE_WITHOUT_PRIMARY_ENTITY.Contains(message))
                        {
                            handleEntities = new string[] { null };
                        }

                        foreach (var logicalname in handleEntities)
                        {
                            var methods = this.pluginMethodCache.ForPlugin(pluginType, stage, message, logicalname, false, false);
                            if (methods != null && methods.Length > 0)
                            {
                                next.AddStep(stage, message, logicalname, false, methods);
                            }
                            if (stage == 40)
                            {
                                methods = this.pluginMethodCache.ForPlugin(pluginType, stage, message, logicalname, true, false);
                                if (methods != null)
                                {
                                    next.AddStep(stage, message, logicalname, true, methods);
                                }
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }
    }
}
