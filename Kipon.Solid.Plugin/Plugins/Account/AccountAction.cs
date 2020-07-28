using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountAction : Kipon.Xrm.BasePlugin
    {
        public void OnPostkipon_AccountCountContacts(Entities.AccountReference target, ServiceAPI.IAccountService accountService, string Name, Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ctx.OutputParameters["Count"] = 0;
            } else
            {
                ctx.OutputParameters["Count"] = Name.Length;
            }
        }
    }
}
