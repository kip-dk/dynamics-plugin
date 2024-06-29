using Kipon.Xrm.Extensions.TypeConverters;
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
        public Actions.AccountCountContactsResponse OnPostkipon_AccountCountContacts(
            Actions.IAccountCountContactsRequest request, 
            ServiceAPI.IAccountService accountService, string Name, 
            Microsoft.Xrm.Sdk.IPluginExecutionContext ctx,
            Entities.IUnitOfWork uow)
        {
            var veid = 2.ToGuid(3);

            var firstve = (from v in uow.Vetests.GetQuery()
                           where v.kipon_vetestId == veid
                           select v).Single();

            if (string.IsNullOrEmpty(Name))
            {
                return new Actions.AccountCountContactsResponse { AMoney = new Money(0M), Count = 0, FirstVeName = firstve.kipon_name };
            } else
            {
                return new Actions.AccountCountContactsResponse { AMoney = new Money(10M), Count = Name.Length, FirstVeName = firstve.kipon_name };
            }
        }
    }
}
