using Kipon.Online.Plugin.Entities;
using Kipon.Online.Plugin.ServiceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Service
{
    public class SpecialAccountService : ServiceAPI.ISpecialAccountService
    {
        private readonly IContactService contactService;

        public SpecialAccountService(
            ServiceAPI.IContactService contactService)
        {
            this.contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        }

        public void x()
        {
        }
    }
}
