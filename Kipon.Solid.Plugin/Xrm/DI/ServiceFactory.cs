using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI
{
    public class ServiceFactory
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();
        private Guid systemuserid;

        private IPluginExecutionContext pluginExecutionContext;
        private IOrganizationServiceFactory organizationServiceFactory;

        public  ServiceFactory(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService, Guid userId)
        {
            this.pluginExecutionContext = pluginExecutionContext;
            this.organizationServiceFactory = organizationServiceFactory;
            this.services.Add(typeof(InvalidPluginExecutionException), pluginExecutionContext);
            this.services.Add(typeof(IOrganizationServiceFactory), organizationServiceFactory);
            this.services.Add(typeof(ITracingService), traceService);
            this.systemuserid = userId;
        }

        public void Execute(Microsoft.Xrm.Sdk.IPlugin plugin)
        {
            var methods = Reflection.PluginMethodCache.ForPlugin(
                plugin.GetType(),
                this.pluginExecutionContext.Stage,
                this.pluginExecutionContext.MessageName, 
                this.pluginExecutionContext.PrimaryEntityName, 
                this.pluginExecutionContext.Mode == 1);

            foreach (var method in methods)
            {
                var objs = new object[method.Parameters == null ? 0 : method.Parameters.Length];
#warning impl. resolve of each parameter to an object instance before invoking methods

                method.Invoke(plugin, objs);

#warning reset state on all services before taking next method to reduce effect from one method to another
            }
        }
    }
}
