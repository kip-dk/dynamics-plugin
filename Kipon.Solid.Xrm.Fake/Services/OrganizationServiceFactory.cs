using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Kipon.Xrm.Fake.Services
{
    public class OrganizationServiceFactory : Microsoft.Xrm.Sdk.IOrganizationServiceFactory
    {
        private Repository.PluginExecutionFakeContext context;

        public OrganizationServiceFactory(Repository.PluginExecutionFakeContext context)
        {
            this.context = context;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return new OrganizationService(context, userId);
        }
    }
}
