using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Attributes;
using Kipon.Dynamics.Plugin.DI;

namespace Kipon.Dynamics.Plugin.Plugins.Contact
{
    [Step(EventType = CrmEventType.Create, PrimaryEntity = Entities.Contact.EntityLogicalName, Stage = StageEnum.PostOperation)]
    public class ContactCpost : AbstractBasePlugin
    {
        protected override void Execute(IPluginContext pluginContext)
        {
            var target = pluginContext.Target.ToEntity<Entities.Contact>();
            pluginContext.GetService<ServiceAPI.IContactService>().UpdateKing(target);
        }
    }
}
