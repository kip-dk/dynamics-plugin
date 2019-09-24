using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Fake
{
    public class OrganizationServiceFactory : Microsoft.Xrm.Sdk.IOrganizationServiceFactory
    {
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return new OrganizationService(userId);
        }
    }
}
