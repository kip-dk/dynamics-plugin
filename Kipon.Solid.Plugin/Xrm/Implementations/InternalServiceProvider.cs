namespace Kipon.Xrm.Implementations
{
    using Reflection;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class InternalServiceProvider : System.IServiceProvider
    {
        private readonly IServiceProvider root;
        private readonly ServiceCache serviceCache;
        private readonly IOrganizationService orgService;

        public InternalServiceProvider(System.IServiceProvider root, Reflection.ServiceCache serviceCache, Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.root = root;
            this.serviceCache = serviceCache;
            this.orgService = orgService;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                var typeCache = Reflection.TypeCache.ForServiceIntereface(serviceType);
                return this.serviceCache.Resolve(typeCache);
            }
            catch (Exception)
            {
                return root.GetService(serviceType);
            }
        }
    }
}
