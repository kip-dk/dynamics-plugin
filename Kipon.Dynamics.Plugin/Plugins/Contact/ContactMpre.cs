using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Attributes;
using Kipon.Dynamics.Plugin.DI;

namespace Kipon.Dynamics.Plugin.Plugins.Contact
{
    [Step(EventType = CrmEventType.Merge, PrimaryEntity = Entities.Contact.EntityLogicalName, Stage = StageEnum.PreOperation)]
    public class ContactMpre : AbstractBasePlugin
    {
        protected override void Execute(IPluginContext pluginContext)
        {
            var ctx = pluginContext.GetService<Microsoft.Xrm.Sdk.IPluginExecutionContext>();

            var uow = pluginContext.GetService<Entities.IUnitOfWork>();

            var pms = ctx.InputParameters;

            var target = ctx.InputParameters["Target"] as Microsoft.Xrm.Sdk.EntityReference;

            var name = (from c in uow.Contacts.GetQuery() where c.ContactId == target.Id select c.FullName).Single();

            var sub = ctx.InputParameters["SubordinateId"];

            var dummy = "x";
        }
    }
}
