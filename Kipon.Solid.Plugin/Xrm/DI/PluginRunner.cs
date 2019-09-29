namespace Kipon.Xrm.DI
{
    using Microsoft.Xrm.Sdk;
    using System.Linq;
    public class PluginRunner
    {
        private Reflection.ServiceCache serviceCache;
        private IPluginExecutionContext pluginExecutionContext;

        public PluginRunner(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService)
        {
            this.pluginExecutionContext = pluginExecutionContext;
            this.serviceCache = new Reflection.ServiceCache(pluginExecutionContext, organizationServiceFactory, traceService);
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
                if (this.pluginExecutionContext.MessageName == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Update.ToString() && !method.FilterAllProperties && method.FilteredProperties != null && method.FilteredProperties.Length > 0)
                {
                    var target = (Microsoft.Xrm.Sdk.Entity)this.pluginExecutionContext.InputParameters["Target"];
                    var doProcess = (from k in target.Attributes.Keys
                                 join a in method.FilteredProperties on k equals a.LogicalName
                                 select k).Any();
                    if (!doProcess)
                    {
                        continue;
                    }
                }

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
