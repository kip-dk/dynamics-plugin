using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Services
{
    public class ServiceProvider : System.IServiceProvider
    {
        private Microsoft.Xrm.Sdk.IPluginExecutionContext context;
        private Microsoft.Xrm.Sdk.IOrganizationServiceFactory orgServiceFactory;
        private Microsoft.Xrm.Sdk.ITracingService traceService;

        public ServiceProvider(Microsoft.Xrm.Sdk.IPluginExecutionContext context, Repository.PluginEntityContext entityContext)
        {
            this.context = context;
            this.orgServiceFactory = new OrganizationServiceFactory(entityContext);
            this.traceService = new TracingService();
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext)) return this.context;
            if (serviceType == typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory)) return this.orgServiceFactory;
            if (serviceType == typeof(Microsoft.Xrm.Sdk.ITracingService)) return this.traceService;

            throw new ArgumentException($"Fake service provider is unable to resolve type: " + serviceType.FullName);
        }
    }
}
