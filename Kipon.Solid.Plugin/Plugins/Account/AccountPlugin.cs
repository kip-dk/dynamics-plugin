using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Attributes;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountPlugin
    {
        public void OnPreCreate(Entities.IAccountNameChanged target, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
        }

        [Sort(100)]
        public void OnPreUpdate(Entities.IAccountNameChanged target, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
        }

        [Sort(101)]
        public void OnPreUpdate(Entities.IAccountSaldoChanged target)
        {
            // do something when changes
        }

    }
}
