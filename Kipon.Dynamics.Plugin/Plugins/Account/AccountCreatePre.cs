using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Attributes;
using Kipon.Dynamics.Plugin.Entities;
using Microsoft.Xrm.Sdk;
using Kipon.Dynamics.Plugin.Extensions.String;

namespace Kipon.Dynamics.Plugin.Plugins.Account
{
    [Step(EventType = CrmEventType.Create, PrimaryEntity = Entities.Account.EntityLogicalName, Stage = StageEnum.PreOperation)]
    public class AccountCreatePre : AbstractBasePlugin
    {
        public override void Execute(IUnitOfWork uow, ITracingService tracingService)
        {
            var target = this.Target.ToEntity<Entities.Account>();
            target.Name = target.Name.UpperCaseWords();
        }
    }
}
