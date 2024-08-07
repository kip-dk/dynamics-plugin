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
            "Merge",
            "GenerateQuoteFromOpportunity",
            "QualifyLead"
        };

        private string[] entityLogicalNames;

        [ImportingConstructor]
        public PluginDeploymentService(ServiceAPI.IMessageService messageService, Entities.IUnitOfWork uow)
        {
            this.messageService = messageService;
            this.uow = uow;
        }

        public Plugin[] PluginForAssembly(Assembly assembly)
        {
            var customActions = (from sm in uow.SdkMessages.GetQuery()
                                 join wf in uow.Workflows.GetQuery() on sm.SdkMessageId equals wf.SdkMessageId.Id
                                 select sm.Name).Distinct().ToArray();

            uow.ClearContext();

            var unbounds = (from sm in uow.SdkMessages.GetQuery()
                            join wf in uow.Workflows.GetQuery() on sm.SdkMessageId equals wf.SdkMessageId.Id
                            where wf.PrimaryEntity == "none"
                              && sm.Name != null
                              && sm.Name != ""
                            select sm.Name).Distinct().ToArray().OrderBy(r => r).ToArray();

            uow.ClearContext();

            this.entityLogicalNames = assembly.GetTypes().
                Where(r => r.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                .Select(r => ((Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(r)).LogicalName)
                .Distinct()
                .ToArray();

            this.messageService.Inform($"Found {this.entityLogicalNames.Length} entities.");


            var x = this.entityLogicalNames;

            var tmp = (from sm in uow.SdkMessages.GetQuery()
                       join wf in uow.Workflows.GetQuery() on sm.SdkMessageId equals wf.SdkMessageId.Id
                       where wf.PrimaryEntity != "none"
                           && sm.Name != null
                           && sm.Name != ""
                       select new
                       {
                           Name = sm.Name,
                           PrimaryEntity = wf.PrimaryEntity
                       }).Distinct()
                         .ToArray();

            var bounds = tmp.Where(r => this.entityLogicalNames != null && this.entityLogicalNames.Length > 0 &&  this.entityLogicalNames.Contains(r.PrimaryEntity))
                         .ToDictionary(r => r.Name, v => v.PrimaryEntity);

            bounds["QualifyLead"] = "lead";
            bounds["GenerateQuoteFromOpportunity"] = "quote";

            uow.ClearContext();

            var allActions = new List<string>(messages);

            allActions.AddRange(customActions);

            messages = allActions.ToArray();

            this.types = Kipon.Tools.Xrm.Reflection.Types.Instance;
            types.SetAssembly(assembly);
            this.pluginMethodCache = new Kipon.Tools.Xrm.Reflection.PluginMethod.Cache(assembly);

            var abstractPlugins = assembly.GetTypes().Where(r => r.BaseType == types.BasePlugin && r.IsAbstract).ToArray();
            var plugins = assembly.GetTypes().Where(r => (r.BaseType == types.BasePlugin && !r.IsAbstract) || abstractPlugins.Contains(r.BaseType)).ToArray();

            var abstractvPlugins = assembly.GetTypes().Where(r => r.BaseType == types.VirtualEntityPlugin && r.IsAbstract).ToArray();
            var vPlugins = assembly.GetTypes().Where(r => (r.BaseType == types.VirtualEntityPlugin && !r.IsAbstract) || abstractvPlugins.Contains(r.BaseType)).ToArray();

            this.messageService.Inform($"Kipon.Xrm.Tools, plugin deployment found {plugins.Length} plugins, virtual entityplugins: {vPlugins.Length} in {assembly.FullName}");

            List<Plugin> result = new List<Plugin>();

            if (vPlugins != null && vPlugins.Length > 0)
            {
                foreach (var s in vPlugins)
                {
                    var next = new Models.Plugin(s, true);
                    result.Add(next);
                }
            }

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
                        else
                        {
                            if (unbounds.Contains(message))
                            {
                                handleEntities = new string[] { null };
                            }
                        }

                        if (bounds.ContainsKey(message))
                        {
                            handleEntities = new string[] { bounds[message] };
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

        public Models.Workflow[] WorkflowForAssembly(System.Reflection.Assembly assembly)
        {
            this.types = Kipon.Tools.Xrm.Reflection.Types.Instance;
            types.SetAssembly(assembly);

            var result = new List<Models.Workflow>();

            var workflowtypes = assembly.GetTypes().Where(r => r.BaseType == types.BaseCodeActivity).ToArray();

            foreach (var workflowtype in workflowtypes)
            {
                var next = new Models.Workflow(workflowtype);
                result.Add(next);
            }


            return result.ToArray();
        }
    }
}
