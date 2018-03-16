using Kipon.Dynamics.Plugin.DI;
using Kipon.Dynamics.Plugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Extensions.String;

namespace Kipon.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IAccountService))]
    public class AccountService : ServiceAPI.IAccountService
    {
        public void UppercaseName(Account target)
        {
            target.Name = target.Name.UpperCaseWords();
        }
    }
}
