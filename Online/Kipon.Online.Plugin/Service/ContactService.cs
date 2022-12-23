using Kipon.Online.Plugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Service
{
    public class ContactService : ServiceAPI.IContactService
    {
        private readonly IQueryable<Contact> contactQuery;
        private readonly IQueryable<Account> accountQuery;

        public ContactService(IQueryable<Entities.Contact> contactQuery, IQueryable<Entities.Account> accountQuery)
        {
            this.contactQuery = contactQuery ?? throw new ArgumentNullException(nameof(contactQuery));
            this.accountQuery = accountQuery ?? throw new ArgumentNullException(nameof(accountQuery));
        }

        public void x()
        {
            var dd = this.contactQuery.FirstOrDefault();
            var yy = this.accountQuery.FirstOrDefault();
        }
    }
}
