using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Entities;
using Kipon.Xrm.Extensions.Strings;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.Service
{
    public class AccountService : ServiceAPI.IAccountService
    {

        public AccountService(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.OrgService = orgService;
        }

        public IOrganizationService OrgService { get; private set; }

        public void OnNameChanged(Account.IAccountNameChanged target)
        {
            if (!string.IsNullOrEmpty(target.Name))
            {
                var names = target.Name.Split(' ').Select(r => r.FirstToUpper());
                target.Name = string.Join(" ", names);
            }
        }
    }
}
