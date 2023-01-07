using Kipon.Online.Plugin.Entities;
using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Xrm.Service
{
    public class SpecialAdminService : ServiceAPI.ISpecialAdminService
    {
        private readonly IUnitOfWork uow;

        public SpecialAdminService([Admin] Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public string UOWImplementation => this.uow.GetType().FullName;
    }
}
