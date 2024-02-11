using Kipon.Xrm.Tools.Models;
using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IAssemblyPluginModelService))]
    public class AssemblyPluginModelService : ServiceAPI.IAssemblyPluginModelService
    {
        private readonly Entities.IUnitOfWork uow;
        private readonly IPluginAssemblyService assmService;
        private readonly IPluginTypeService typeService;
        private readonly ISdkMessageProcessingStepService stepService;

        [ImportingConstructor]
        public AssemblyPluginModelService(
            Entities.IUnitOfWork uow, 
            ServiceAPI.IPluginAssemblyService assmService,
            ServiceAPI.IPluginTypeService typeService,
            ServiceAPI.ISdkMessageProcessingStepService stepService)
        {
            if (assmService is null)
            {
                throw new ArgumentNullException(nameof(assmService));
            }

            this.uow = uow;
            this.assmService = assmService;
            this.typeService = typeService;
            this.stepService = stepService;
        }

        public Models.PluginAssembly Get(string name)
        {
            var assm = this.assmService.GetPluginAssembly(name);
            var result = new Models.PluginAssembly
            {
                Id = assm.PluginAssemblyId.Value,
                Name = assm.Name
            };

            var types = this.typeService.GetPluginTypes(assm.PluginAssemblyId.Value);
            var steps = this.stepService.ForPluginAssembly(assm.PluginAssemblyId.Value);
            var filters = this.stepService.FiltersForAssembly(assm.PluginAssemblyId.Value); 
            var images = this.stepService.ImagesForPluginAssembly(assm.PluginAssemblyId.Value);

            result.PluginTypes = (from t in types
                                  select new Models.PluginAssembly.PluginType
                                  {
                                      Id = t.PluginTypeId.Value,
                                      Name = t.Name
                                  }).ToArray();

            foreach (var type in result.PluginTypes)
            {
                type.PluginSteps = (from step in steps
                                    where step.EventHandler.Id == type.Id
                                    select new Models.PluginAssembly.PluginStep
                                    {
                                        Id = step.SdkMessageProcessingStepId.Value,
                                        Name = step.Name,
                                        Stage = step.Stage.Value,
                                        Mode = step.Mode.Value,
                                        Message = step.SdkMessageId.Name,
                                        FilteringAttributes = step.FilteringAttributes,
                                        Rank = step.Rank.Value,
                                        PrimaryEntityLogicalName = filters.Where(r => step.SdkMessageFilterId != null &&  r.SdkMessageFilterId == step.SdkMessageFilterId.Id).SingleOrDefault()?.PrimaryObjectTypeCode
                                    }).ToArray();

                foreach (var step in type.PluginSteps)
                {
                    step.Images = (from i in images
                                   where i.SdkMessageProcessingStepId.Id == step.Id
                                   select new Models.PluginAssembly.Image
                                   {
                                       Id = i.SdkMessageProcessingStepImageId.Value,
                                       Name = i.Name,
                                       Attributes1 = i.Attributes1,
                                       Description = i.Description,
                                       EntityAlias = i.EntityAlias,
                                       ImageType = i.ImageType.Value,
                                       MessagePropertyName = i.MessagePropertyName
                                   }).ToArray();
                }
            }
            return result;
        }

        public void Export(string assmName, string filename)
        {
            var model = this.Get(assmName);

            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Models.PluginAssembly));
            using (var file = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            {
                ser.WriteObject(file, model);
            }
        }


        public void Import(string filename)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Models.PluginAssembly));
            using (var file = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                var importModel = (Models.PluginAssembly)ser.ReadObject(file);
                var crmModel = this.Get(importModel.Name);

                var sdkmessageIndex = (from s in uow.SdkMessages.GetQuery()
                                       select s).ToDictionary(r => r.Name);

                foreach (var type in importModel.PluginTypes)
                {
                    var crmType = crmModel.PluginTypes.Where(r => r.Name == type.Name).SingleOrDefault();
                    if (crmType != null)
                    {
                        foreach (var step in type.PluginSteps)
                        {
                            var crmStep = (from s in crmType.PluginSteps
                                           where s.Stage == step.Stage
                                             && s.Mode == step.Mode
                                             && s.Message == step.Message
                                           select s).SingleOrDefault();

                            if (crmStep == null)
                            {
                                var trans = new Services.CrmTransaction();

                                var newStep = new Entities.SdkMessageProcessingStep
                                {
                                    SdkMessageProcessingStepId = step.Id,
                                    Name = step.Name,
                                    Mode = new Microsoft.Xrm.Sdk.OptionSetValue(step.Mode),
                                    Rank = step.Rank,
                                    Stage = new Microsoft.Xrm.Sdk.OptionSetValue(step.Stage),
                                    SupportedDeployment = new Microsoft.Xrm.Sdk.OptionSetValue(0),
                                    EventHandler = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginType.EntityLogicalName, crmType.Id),
                                    SdkMessageId = sdkmessageIndex[step.Message].ToEntityReference(),
                                    FilteringAttributes = step.FilteringAttributes
                                };

                                if (step.PrimaryEntityLogicalName != null)
                                {
                                    newStep.SdkMessageFilterId = this.stepService.GetFilterFor(sdkmessageIndex[step.Message], step.PrimaryEntityLogicalName);
                                }

                                if (newStep.Mode.Value == 1)
                                {
                                    newStep.AsyncAutoDelete = true;
                                }


                                trans.Create(newStep);

                                if (step.Images != null && step.Images.Length > 0)
                                {
                                    foreach (var image in step.Images)
                                    {
                                        var newImage = new Entities.SdkMessageProcessingStepImage
                                        {
                                            SdkMessageProcessingStepImageId = image.Id,
                                            SdkMessageProcessingStepId = new Microsoft.Xrm.Sdk.EntityReference(Entities.SdkMessageProcessingStep.EntityLogicalName, newStep.SdkMessageProcessingStepId.Value),
                                            Name = image.Name,
                                            EntityAlias = image.EntityAlias,
                                            Description = image.Description,
                                            ImageType = new Microsoft.Xrm.Sdk.OptionSetValue(image.ImageType),
                                            MessagePropertyName = image.MessagePropertyName,
                                        };

                                        if (!string.IsNullOrEmpty(image.Attributes1))
                                        {
                                            newImage.Attributes1 = image.Attributes1;
                                        }
                                        trans.Create(newImage);
                                    }
                                }

                                trans.Commit(this.uow);
                                Console.WriteLine($"Step: { step.Name } for type: {crmType.Name} was created");
                            } else
                            {
                                Console.WriteLine($"Step: { step.Name } for Type: { crmType.Name } already exists");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Type: { type.Name } does not exists in target environment");
                    }
                }
            }
        }
    }
}
