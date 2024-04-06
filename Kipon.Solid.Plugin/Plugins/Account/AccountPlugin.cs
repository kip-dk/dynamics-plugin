using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountPlugin : MyProjectSpecificBasePlugin
    {

        public void OnValidateCreate(Entities.Account.IAccountMergedImageWithTargetAttributes target, ServiceAPI.IAccountService accountService)
        {
        }

        public void OnPreCreate(Entities.Account.IAccountNameChanged target, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
        }


        [Sort(101)]
        public void OnPreUpdate(Entities.Account.ICreditLimitChanged target)
        {
            if (Setting.IsUnitTest)
            {
                if (target.CreditLimit == null)
                {
                    target.CreditLimit = new Microsoft.Xrm.Sdk.Money(100M);
                }
            }
        }

        [Sort(100)]
        public void OnPreUpdate(Entities.Account.IAccountNameChanged target, Entities.Account.IAccountPreName prename, ServiceAPI.IAccountService accountService)
        {
            accountService.OnNameChanged(target);
            target.setDescription(prename.Name);
        }


        [Sort(102)]
        public void OnPreUpdate(Entities.Account.IAccountNameAndSettersOnly target)
        {
        }


        public void OnPreDelete(Entities.AccountReference accountRef)
        {
        }

    }
}
