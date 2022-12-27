using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class AccountCreatePlugin : Kipon.Xrm.BasePlugin
    {
        [Sort(1)]
        public void OnValidateCreate(Entities.Account target, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace("t1");
            if (Setting.IsUnitTest)
            {
                if (target != null && target.CreditLimit == null)
                {
                    target.CreditLimit = new Microsoft.Xrm.Sdk.Money(100M);
                }
            }
            traceService.Trace("t2");
        }

        [Sort(2)]
        public void OnValidateCreate(Entities.Account.IAccountNameChanged target, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace("t3");
            if (Setting.IsUnitTest)
            {
                if (target.Name != null && target.Name.StartsWith("kurt"))
                {
                    target.Name = "Jens";
                }
            }
            traceService.Trace("t4");
        }
    }
}
