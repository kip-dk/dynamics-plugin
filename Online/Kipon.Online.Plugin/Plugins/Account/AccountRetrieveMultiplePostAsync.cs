using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class AccountRetrieveMultiplePostAsync : Kipon.Xrm.BasePlugin
    {
        [LogicalName(Entities.Account.EntityLogicalName)]
        public void OnPostRetrieveMultipleAsync()
        {
        }
    }
}
