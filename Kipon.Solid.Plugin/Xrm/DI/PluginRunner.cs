namespace Kipon.Xrm.DI
{
    using Microsoft.Xrm.Sdk;
    using Kipon.Xrm.Extensions.Sdk;
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
            var methods = Reflection.PluginMethod.ForPlugin(
                plugin.GetType(),
                this.pluginExecutionContext.Stage,
                this.pluginExecutionContext.MessageName, 
                this.pluginExecutionContext.PrimaryEntityName, 
                this.pluginExecutionContext.Mode == 1);

            foreach (var method in methods)
            {
                #region find out if method is relevant at all
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
                #endregion

                #region create statement, target images can define required fields. Validate that these rules are respected
                if (this.pluginExecutionContext.MessageName == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Create.ToString() && (this.pluginExecutionContext.Stage == 10 || this.pluginExecutionContext.Stage == 20) && method.HasRequiredProperties)
                {
                    var requiredProperties = (from p in method.FilteredProperties where p.Required == true select p).ToArray();
                    var target = (Microsoft.Xrm.Sdk.Entity)this.pluginExecutionContext.InputParameters["Target"];
                    foreach (var rp in requiredProperties)
                    {
                        if (target.GetSafeValue(rp.LogicalName) == null)
                        {
                            throw new InvalidPluginExecutionException($"Attribute {rp.LogicalName} on {target.LogicalName} is required and cannot be omitted or assigned null in Create message.");
                        }
                    }
                }
                #endregion

                #region update statement, target image can define required fields. Validate that no required properties are assigned the value null
                if (this.pluginExecutionContext.MessageName == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Update.ToString() && (this.pluginExecutionContext.Stage == 10 || this.pluginExecutionContext.Stage == 20) && method.HasRequiredProperties)
                {
                    var requiredProperties = (from p in method.FilteredProperties where p.Required == true select p).ToArray();
                    var target = (Microsoft.Xrm.Sdk.Entity)this.pluginExecutionContext.InputParameters["Target"];
                    foreach (var rp in requiredProperties)
                    {
                        if (target.Attributes.Contains(rp.LogicalName) && target.GetSafeValue(rp.LogicalName) == null)
                        {
                            throw new InvalidPluginExecutionException($"Attribute {rp.LogicalName} on {target.LogicalName} is required and cannot be assigned null in Update message.");
                        }
                    }
                }
                #endregion


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
