using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Attributes;
using Kipon.Dynamics.Plugin.DI;

namespace Kipon.Dynamics.Plugin.Plugins.Contact
{
    [Step(EventType = CrmEventType.Update, 
        PrimaryEntity = Entities.Contact.EntityLogicalName, 
        Stage = StageEnum.PostOperation, Preimage = true)]
    public class ContactUpost : AbstractBasePlugin
    {
        protected override void Execute(IPluginContext pluginContext)
        {
            /*
            if (pluginContext.AttributeChanged("jobtitle"))
            {
                var full = pluginContext.GetFullImage().ToEntity<Entities.Contact>();
                var target = pluginContext.Target.ToEntity<Entities.Contact>();
                pluginContext.GetService<ServiceAPI.IContactService>().UpdateKing(full, target.JobTitle);
            }
            */
        }
    }
}
