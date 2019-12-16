using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountCreatePlugin : Kipon.Xrm.BasePlugin
    {
        [Sort(1)]
        public void OnValidateCreate(Entities.Account target, Entities.IUnitOfWork uow)
        {
            if (target != null && target.CreditLimit == null)
            {
                target.CreditLimit = new Microsoft.Xrm.Sdk.Money(100M);
            }
        }

        [Sort(2)]
        public void OnValidateCreate(Entities.Account.IAccountNameChanged account)
        {
            if (account.Name != null && account.Name.StartsWith("kurt"))
            {
                account.Name = "Jens";
            } 
        }
    }
}
