using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Kipon.Xrm.Fake.Services
{
    public class OrganizationServiceFactory : Microsoft.Xrm.Sdk.IOrganizationServiceFactory, Microsoft.Xrm.Sdk.IProxyTypesAssemblyProvider
    {
        private Repository.PluginExecutionFakeContext context;

        public OrganizationServiceFactory(Repository.PluginExecutionFakeContext context)
        {
            this.context = context;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            var r = new OrganizationService(context, userId);
            var p = r as Microsoft.Xrm.Sdk.IProxyTypesAssemblyProvider;

            if (p != null)
            {
                p.ProxyTypesAssembly = this.ProxyTypesAssembly;
            }

            return r;
        }

        public Assembly ProxyTypesAssembly { get; set; }

    }
}
