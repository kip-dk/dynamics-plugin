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

        public PluginType[] ForPluginAssembly(Guid pluginAssemblyId)
        {
            return (from pt in uow.PluginTypes.GetQuery()
                    where pt.PluginAssemblyId.Id == pluginAssemblyId
                    select pt).ToArray();
        }

        public void JoinAndCleanup(Entities.PluginType[] currents, Models.Plugin[] tobee)
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

        public void CreateAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees)
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
                    };
                    uow.Create(next);
                    tobee.CurrentCrmInstance = next;
                }
            }
        }

        public void FindAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees)
        {
            foreach (var tobee in tobees)
            {
                if (tobee.CurrentCrmInstance == null)
                {
                    tobee.CurrentCrmInstance = (from pt in uow.PluginTypes.GetQuery()
                                                where pt.PluginAssemblyId.Id == pluginassemblyId
                                                  && pt.Name == tobee.Type.FullName
                                                select pt).SingleOrDefault();

                    if (tobee.CurrentCrmInstance == null)
                    {
                        throw new Exception($"Plugin Type with name: { tobee.Type.FullName } was not found. That is unexpected.");
                    }
                }
            }
        }


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
