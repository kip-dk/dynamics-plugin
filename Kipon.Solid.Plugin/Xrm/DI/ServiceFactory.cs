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
        private Reflection.ServiceCache serviceCache;
        private IPluginExecutionContext pluginExecutionContext;

        public ServiceFactory(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService, Guid userId)
        {
            this.pluginExecutionContext = pluginExecutionContext;
            this.serviceCache = new Reflection.ServiceCache(pluginExecutionContext, organizationServiceFactory, traceService, userId);
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

                var ix = 0;
                foreach (var type in method.Parameters)
                {
                    objs[ix] = serviceCache.Resolve(type);
                }

                method.Invoke(plugin, objs);

#warning reset state on all services before taking next method to reduce effect from one method to another
            }
        }
    }
}
