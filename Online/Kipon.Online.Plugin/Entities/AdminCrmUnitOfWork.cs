using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class AdminCrmUnitOfWork
    {
        public Microsoft.Xrm.Sdk.IOrganizationService OrgService => this._service;
    }
}
