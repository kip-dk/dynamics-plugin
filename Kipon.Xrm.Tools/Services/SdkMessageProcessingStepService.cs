using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Entities;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.ISdkMessageProcessingStepService))]
    public class SdkMessageProcessingStepService : ServiceAPI.ISdkMessageProcessingStepService
    {

        private Entities.IUnitOfWork uow;
        private ServiceAPI.IMessageService messageService;

        [ImportingConstructor]
        public SdkMessageProcessingStepService(Entities.IUnitOfWork uow, ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        public SdkMessageProcessingStep[] ForPluginAssembly(Guid pluginassemblyid)
        {
            return (from sm in uow.SdkMessageProcessingSteps.GetQuery()
                    join pt in uow.PluginTypes.GetQuery() on sm.EventHandler.Id equals pt.PluginTypeId
                    where pt.PluginAssemblyId.Id == pluginassemblyid
                    select sm).ToArray();
        }


        public Entities.SdkMessageProcessingStep[] Cleanup(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins)
        {
            List<Entities.SdkMessageProcessingStep> remains = new List<SdkMessageProcessingStep>();

            foreach (var plugin in plugins)
            {
                if (plugin.CurrentCrmInstance != null)
                {
                    foreach (var step in plugin.Steps)
                    {
                        var name = plugin.NameOf(step.Stage, step.Message, step.IsAsync, step.PrimaryEntityLogicalName);
                        var exists = (from s in steps
                                      where s.Name == name
                                      select s).ToArray();

                        if (exists.Length > 0)
                        {
                            remains.Add(exists[0]);
                        }
                    }
                }
            }

            var removes = (from s in steps where !remains.Contains(s) select s).ToArray();
            foreach (var remove in removes)
            {
                uow.Delete(remove);
            }

            return remains.ToArray();
        }

        public Entities.SdkMessageProcessingStep[] CreateOrUpdateSteps(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins)
        {
            var result = new List<SdkMessageProcessingStep>();

            foreach (var plugin in plugins)
            {
                foreach (var step in plugin.Steps)
                {
                    var name = plugin.NameOf(step.Stage, step.Message, step.IsAsync, step.PrimaryEntityLogicalName);
                    var crmStep = (from s in steps where s.Name == name select s).SingleOrDefault();

                    if (crmStep == null)
                    {
                        result.Add(this.Create(plugin, step, name));
                    }
                    else
                    {
                        result.Add(crmStep);
                        this.Update(plugin, step, crmStep);
                    }
                }
            }
            return result.ToArray();
        }

        private void Update(Models.Plugin plugin, Models.Step step, Entities.SdkMessageProcessingStep crmStep)
        {
            var updated = false;
            var clean = new Entities.SdkMessageProcessingStep
            {
                SdkMessageProcessingStepId = crmStep.SdkMessageProcessingStepId
            };

            if (crmStep.FilteringAttributes != null && step.FilteringAttributesString == null)
            {
                clean.FilteringAttributes = null;
                updated = true;
            }

            if (step.FilteringAttributesString != null && step.FilteringAttributesString != crmStep.FilteringAttributes)
            {
                clean.FilteringAttributes = step.FilteringAttributesString;
                updated = true;
            }

            if (updated)
            {
                uow.Update(clean);
            }

            this.UpdateImage(crmStep, 1, step.Stage, step.IsAsync, step.Message, step.PreImage);
            this.UpdateImage(crmStep, 2, step.Stage, step.IsAsync, step.Message, step.PostImage);
        }

        private Entities.SdkMessageProcessingStep Create(Models.Plugin plugin, Models.Step step, string name)
        {
            var next = new Entities.SdkMessageProcessingStep
            {
                SdkMessageProcessingStepId = Guid.NewGuid(),
                Name = name,
                Mode = step.IsAsync ? new Microsoft.Xrm.Sdk.OptionSetValue(1) : new Microsoft.Xrm.Sdk.OptionSetValue(0),
                Rank = step.ExecutionOrder <= 0 ? 1 : step.ExecutionOrder,
                Stage = new Microsoft.Xrm.Sdk.OptionSetValue(step.Stage),
                SupportedDeployment = new Microsoft.Xrm.Sdk.OptionSetValue(0),
                EventHandler = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginType.EntityLogicalName, plugin.CurrentCrmInstance.PluginTypeId.Value),
                SdkMessageId = this.GetSdkMessage(step.Message).ToEntityReference(),
                SdkMessageFilterId = this.GetFilterFor(this.GetSdkMessage(step.Message), step.PrimaryEntityLogicalName)
            };

            if (next.Mode.Value == 1)
            {
                next.AsyncAutoDelete = true;
            }

            if (step.FilteringAttributes != null && step.FilteringAttributes.Length > 0)
            {
                next.FilteringAttributes = string.Join(",", step.FilteringAttributes);
            }

            uow.Create(next);
            this.messageService.Inform($"Created step: {next.Name}");

            if (step.PreImage != null)
            {
                CreateImage(next, 1, step.Stage, step.IsAsync, step.Message, step.PreImage);
            }

            if (step.PostImage != null)
            {
                CreateImage(next, 2, step.Stage, step.IsAsync, step.Message, step.PostImage);
            }
            return next;
        }

        private void UpdateImage(SdkMessageProcessingStep crmStep, int pre1post2, int stage, bool async, string message, Models.Image imgDef)
        {
            var name = Kipon.Tools.Xrm.Reflection.PluginMethod.ImageSuffixFor(pre1post2, stage, async);

            var existingImage = (from ig in uow.SdkMessageProcessingStepImages.GetQuery()
                                 where ig.SdkMessageProcessingStepId.Id == crmStep.SdkMessageProcessingStepId
                                   && ig.Name == name
                                 select ig).SingleOrDefault();

            if (imgDef == null)
            {
                if (existingImage != null)
                {
                    uow.Delete(existingImage);
                    this.messageService.Inform($"Removed images {name} from {crmStep.Name}");
                }
                return;
            }

            if (existingImage == null)
            {
                this.CreateImage(crmStep, pre1post2, stage, async, message, imgDef);
            }
            else
            {
                string filterAttr = null;
                if (!imgDef.AllAttributes && imgDef.FilteredAttributes != null && imgDef.FilteredAttributes.Length > 0)
                {
                    filterAttr = string.Join(",", imgDef.FilteredAttributes);
                }

                if (existingImage.Attributes1 == null && filterAttr == null)
                {
                    return;
                }

                if (existingImage.Attributes1 != null && filterAttr == null)
                {
                    var clean = new Entities.SdkMessageProcessingStepImage { SdkMessageProcessingStepImageId = existingImage.SdkMessageProcessingStepImageId };
                    clean.Attributes1 = null;
                    uow.Update(clean);
                    this.messageService.Inform($"Removed filtering on images {name} from {crmStep.Name}");
                    return;
                }

                if (existingImage.Attributes1 != filterAttr)
                {
                    var clean = new Entities.SdkMessageProcessingStepImage { SdkMessageProcessingStepImageId = existingImage.SdkMessageProcessingStepImageId };
                    clean.Attributes1 = filterAttr;
                    uow.Update(clean);
                    this.messageService.Inform($"Updated filtering on images {name} from {crmStep.Name}, {filterAttr}");
                    return;
                }
            }
        }

        private void CreateImage(SdkMessageProcessingStep crmStep, int pre1post2, int stage, bool async, string message, Models.Image imgDef)
        {
            var name = Kipon.Tools.Xrm.Reflection.PluginMethod.ImageSuffixFor(pre1post2, stage, async);
            var image = new SdkMessageProcessingStepImage
            {
                SdkMessageProcessingStepImageId = Guid.NewGuid(),
                SdkMessageProcessingStepId = new Microsoft.Xrm.Sdk.EntityReference(Entities.SdkMessageProcessingStep.EntityLogicalName, crmStep.SdkMessageProcessingStepId.Value),
                Name = name,
                EntityAlias = name,
                Description = name,
                ImageType = new Microsoft.Xrm.Sdk.OptionSetValue((pre1post2 - 1)),
                MessagePropertyName = this.MessagePropertyName(message),
            };

            string filterAttr = null;
            if (!imgDef.AllAttributes && imgDef.FilteredAttributes != null && imgDef.FilteredAttributes.Length > 0)
            {
                filterAttr = string.Join(",", imgDef.FilteredAttributes);
            }

            if (!string.IsNullOrEmpty(filterAttr))
            {
                image.Attributes1 = filterAttr;
            }

            uow.Create(image);
            messageService.Inform($"Created image {name} on step {crmStep.Name}");
        }

        private Dictionary<string, SdkMessage> sdkmessages;
        private Entities.SdkMessage GetSdkMessage(string message)
        {
            if (this.sdkmessages == null)
            {
                this.sdkmessages = (from s in uow.SdkMessages.GetQuery()
                                    select s).ToDictionary(r => r.Name);
            }
            return this.sdkmessages[message];
        }

        private Dictionary<string, SdkMessageFilter> filters = new Dictionary<string, SdkMessageFilter>();
        private Microsoft.Xrm.Sdk.EntityReference GetFilterFor(SdkMessage sdkMessage, string logicalname)
        {
            var key = $"{sdkMessage.SdkMessageId.Value.ToString()}.{logicalname}";

            if (filters.ContainsKey(key))
            {
                var v = filters[key];
                if (v != null)
                {
                    return v.ToEntityReference();
                } else
                {
                    return null;
                }
            }

            filters[key] = (from f in uow.SdkMessageFilters.GetQuery()
                            where f.SdkMessageId.Id == sdkMessage.SdkMessageId
                               && f.PrimaryObjectTypeCode == logicalname
                               && f.SecondaryObjectTypeCode == null
                            select f).SingleOrDefault();

            return GetFilterFor(sdkMessage, logicalname);
        }


        private string MessagePropertyName(string message)
        {
            switch (message)
            {
                case "Associate":
                case "Disassociate":
                case "SetState":
                case "SetStateDynamicEntity":
                case "Close":
                    return "EntityMoniker";
                case "Delete":
                case "Update":
                    return "Target";
                case "Create":
                    return "Id";
                default: throw new ArgumentException("MessagePropertyName has not been maped for " + message);
            }
        }

    }
}
