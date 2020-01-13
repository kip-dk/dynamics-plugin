using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Contact
{
    public class ContactDeletePlugin : Kipon.Xrm.BasePlugin
    {

        public static readonly Guid TESTID = Guid.NewGuid();

        public void OnPreDelete(Entities.ContactReference target, Model.INamed preimage)
        {
            if (target.Value.Id != TESTID)
            {
                throw new Exception("Id was not as expected");
            }

            if (target.Value.LogicalName != Entities.Contact.EntityLogicalName)
            {
                throw new Exception("logical name was not as expected");
            }

            if (preimage.Name != "solid test")
            {
                throw new Exception("name is not solid test as expected in test.");
            }
        }

        public void OnPreDelete(Entities.AccountReference target, Model.INamed preimage)
        {
            if (target.Value.Id != TESTID)
            {
                throw new Exception("Id was not as expected");
            }

            if (target.Value.LogicalName != Entities.Account.EntityLogicalName)
            {
                throw new Exception("logical name was not as expected");
            }

            if (preimage.Name != "solid test")
            {
                throw new Exception("name is not solid test as expected in test.");
            }
        }

    }
}
