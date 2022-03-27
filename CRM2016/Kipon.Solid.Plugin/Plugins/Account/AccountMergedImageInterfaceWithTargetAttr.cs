using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountMergedImageInterfaceWithTargetAttr : Kipon.Xrm.BasePlugin
    {
        public void OnPreUpdate(Entities.Account.IAccountMergedImageWithTargetAttributes account)
        {
            account.Description = account.AccountNumber + "|" + account.Name;
        }
    }
}
