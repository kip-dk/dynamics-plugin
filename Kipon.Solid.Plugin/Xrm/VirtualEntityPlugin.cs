namespace Kipon.Xrm
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;

    public class VirtualEntityPlugin : Microsoft.Xrm.Sdk.IPlugin
    {
        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }

        internal static readonly Reflection.PluginMethod.Cache PluginMethodCache;

        static VirtualEntityPlugin()
        {
            PluginMethodCache = new Reflection.PluginMethod.Cache(typeof(BasePlugin).Assembly);
            Reflection.Types.Instance.SetAssembly(typeof(BasePlugin).Assembly);
        }

        #region constructors
        public VirtualEntityPlugin() : base()
        {
        }

        public VirtualEntityPlugin(string unSecure, string secure) : this()
        {
            this.UnsecureConfig = unSecure;
            this.SecureConfig = secure;
        }
        #endregion


        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var userId = context.UserId;
            var message = context.MessageName;

            if (message != "Retrieve" && message != "RetrieveMultiple")
            {
                throw new InvalidPluginExecutionException($"Unsupported message in VirtualEntityPlugin { message }. Only Retrieve and RetrieveMultiple is supported");
            }

            var type = (CrmEventType)Enum.Parse(typeof(CrmEventType), context.MessageName);

            IPluginContext pluginContext = new Services.PluginContext(this.UnsecureConfig, this.SecureConfig, context, type, userId);

            IOrganizationService toolOrgService = null;

            if (type == CrmEventType.Retrieve || type == CrmEventType.RetrieveMultiple)
            {
                toolOrgService = serviceFactory.CreateOrganizationService(null);
            }

            using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext))
            {
                var method = PluginMethodCache.ForPlugin(this.GetType(), 30, message, null, context.Mode == 1).First();
                var args = new object[method.Parameters.Length];

                var ix = 0;
                foreach (var p in method.Parameters)
                {
                    args[ix] = serviceCache.Resolve(p,toolOrgService);
                    ix++;
                }

                var result = method.Invoke(this, args);
                if (result != null)
                {
                    if (message == "Retrieve")
                    {
                        var be = result as Microsoft.Xrm.Sdk.Entity;
                        if (be == null)
                        {
                            throw new InvalidPluginExecutionException("Return from virtual antity Retrieve must be of type Microsoft.Xrm.Sdk.Entity");
                        }
                        context.OutputParameters["BusinessEntity"] = be;
                    }

                    if (message == "RetrieveMultiple")
                    {
                        var bes = result as Microsoft.Xrm.Sdk.EntityCollection;
                        if (bes == null)
                        {
                            throw new InvalidPluginExecutionException("Return from virtual entity RetrieveMultiple must be of type Microsoft.Xrm.Sdk.EntityCollection");
                        }
                        context.OutputParameters["BusinessEntityCollection"] = bes;
                    }
                }
            }
        }
    }
}
