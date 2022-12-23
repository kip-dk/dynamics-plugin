using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Online.Plugin.Plugins.Generic
{
    public class ProspectPlugin : Kipon.Xrm.BasePlugin
    {
        [LogicalName(Entities.Account.EntityLogicalName)]
        [LogicalName(Entities.Contact.EntityLogicalName)]
        public void OnPreUpdate(Model.IProspect target)
        {
            switch (target.LogicalName)
            {
                case Entities.Account.EntityLogicalName:
                    {
                        var account = (Entities.Account)target;
                        account.CreditLimit = new Microsoft.Xrm.Sdk.Money(-99M);
                        break;
                    }
                case Entities.Contact.EntityLogicalName:
                    {
                        var contact = (Entities.Contact)target;
                        contact.CreditLimit = new Microsoft.Xrm.Sdk.Money(-97M);
                        break;
                    }
            }
        }
    }
}
