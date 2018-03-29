using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.DI;
using Kipon.Dynamics.Plugin.Entities;

namespace Kipon.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IContactService))]
    public class ContactService : ServiceAPI.IContactService
    {
        [Import]
        public IUnitOfWork uow { get; set; }

        [Import]
        public ServiceAPI.IAccountService accountService { get; set; }

        public void UpdateKing(Contact target, string jobtitle = null)
        {
            if (jobtitle == null) jobtitle = target.JobTitle;
 
            if (jobtitle != null && jobtitle.ToUpper() == "KING") {
                var currentKings = (from c in uow.Contacts.GetQuery()
                                    where c.ContactId != target.ContactId
                                      && c.JobTitle == "King"
                                    select c).ToArray();

                foreach (var contact in currentKings)
                {
                    var clean = uow.Contacts.Clean(contact);
                    clean.JobTitle = "Citicen";
                    uow.Update(clean);
                }

                var opp = new Opportunity
                {
                    CustomerId = new Microsoft.Xrm.Sdk.EntityReference(Entities.Contact.EntityLogicalName, target.ContactId.Value),
                    Name = target.FullName
                };
                uow.Create(opp);
            }

        }
    }
}
