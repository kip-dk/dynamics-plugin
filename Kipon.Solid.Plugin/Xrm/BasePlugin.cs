namespace Kipon.Xrm
{
    using System;
    using Microsoft.Xrm.Sdk;
    public class BasePlugin : IPlugin
    {
        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }

        internal static Reflection.PluginMethod.Cache PluginMethodCache;

        static BasePlugin()
        {
            PluginMethodCache = new Reflection.PluginMethod.Cache(typeof(BasePlugin).Assembly);
            Reflection.Types.Instance.SetAssembly(typeof(BasePlugin).Assembly);
        }

        #region constructors
        public BasePlugin() : base()
        {
        }

        public BasePlugin(string unSecure, string secure) : this()
        {
            this.UnsecureConfig = unSecure;
            this.SecureConfig = secure;
        }
        #endregion

        #region iplugin impl
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var userId = context.UserId;
            var message = context.MessageName;
            var stage = context.Stage;
            var isAsync = context.Mode == 1;

            var type = (CrmEventType)Enum.Parse(typeof(CrmEventType), context.MessageName);

            IPluginContext pluginContext = new Services.PluginContext(this.UnsecureConfig, this.SecureConfig, context, type, userId);

            using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext))
            {
                var methods = PluginMethodCache.ForPlugin(this.GetType(), stage, message, context.PrimaryEntityName, context.Mode == 1);

                foreach (var method in methods)
                {
                    #region find out if method is relevant, looking a target fields
                    if (message == Attributes.StepAttribute.MessageEnum.Update.ToString() && !method.FilterAllProperties)
                    {
                        var target = (Microsoft.Xrm.Sdk.Entity)context.InputParameters["Target"];
                        if (!method.IsRelevant(target))
                        {
                            continue;
                        }
                    }
                    #endregion

                    #region now resolve all parameters
                    var args = new object[method.Parameters.Length];
                    var ix = 0;
                    foreach (var p in method.Parameters)
                    {
                        args[ix] = serviceCache.Resolve(p);
                        ix++;
                    }
                    #endregion

                    #region run the method
                    method.Invoke(this, args);
                    #endregion

                    #region prepare for next method
                    serviceCache.OnStepFinalize();
                    #endregion
                }
            }
        }
        #endregion
    }
}
