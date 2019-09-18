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

        public  ServiceFactory(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFaktory, ITracingService traceService, Guid userId)
        {
            this.services.Add(typeof(InvalidPluginExecutionException), pluginExecutionContext);
            this.services.Add(typeof(IOrganizationServiceFactory), organizationServiceFaktory);
            this.services.Add(typeof(ITracingService), traceService);
            this.systemuserid = userId;
        }

        public object[] ResolveParameters(System.Reflection.MethodInfo method)
        {
            var methodType = Reflection.MethodArgumentCache.ForMethod(method);
            var result = new object[methodType.Types.Length];

            foreach (var type in methodType.Types)
            {
            }
            return result;
        }
    }
}
