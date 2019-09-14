using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountPlugin
    {
        public void OnPreCreate(Entities.IAccountNameChanged target, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
        }

        public void OnPreUpdate(Entities.IAccountNameChanged target, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
        }
    }
}
