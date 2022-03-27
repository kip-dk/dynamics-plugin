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

            using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext, this.UnsecureConfig, this.SecureConfig))
            {
                var method = PluginMethodCache.ForPlugin(this.GetType(), 30, message, context.PrimaryEntityName, context.Mode == 1).Single();

                var args = new object[method.Parameters.Length];

                var ix = 0;

                Microsoft.Xrm.Sdk.Entity datasource = null;

                foreach (var p in method.Parameters)
                {
                    if (p.Name != null && p.Name.ToLower() == "datasource")
                    {
                        if (datasource == null)
                        {
                            throw new NotImplementedException();
                        }
                        args[ix] = datasource;
                    }
                    else
                    {
                        args[ix] = serviceCache.Resolve(p, toolOrgService);
                    }
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

                        if (be.GetType() != typeof(Microsoft.Xrm.Sdk.Entity))
                        {
                            var fr = new Microsoft.Xrm.Sdk.Entity { Id = be.Id, LogicalName = be.LogicalName };
                            fr.Attributes = be.Attributes;
                            be = fr;
                        }

                        this.RemoveNullValues(be);
                        context.OutputParameters["BusinessEntity"] = be;
                    }

                    if (message == "RetrieveMultiple")
                    {
                        var bes = result as Microsoft.Xrm.Sdk.EntityCollection;
                        if (bes == null)
                        {
                            throw new InvalidPluginExecutionException("Return from virtual entity RetrieveMultiple must be of type Microsoft.Xrm.Sdk.EntityCollection");
                        }

                        var entities = bes.Entities.ToArray();
                        bes.Entities.Clear();


                        foreach (var be in entities)
                        {
                            if (be.GetType() == typeof(Microsoft.Xrm.Sdk.Entity))
                            {
                                bes.Entities.Add(be);
                            } else
                            {
                                var fr = new Microsoft.Xrm.Sdk.Entity { Id = be.Id, LogicalName = be.LogicalName };
                                fr.Attributes = be.Attributes;
                                bes.Entities.Add(fr);
                            }
                        }

                        foreach (var fe in bes.Entities)
                        {
                            this.RemoveNullValues(fe);
                        }
                        context.OutputParameters["BusinessEntityCollection"] = bes;
                    }
                }
            }
        }

        private void RemoveNullValues(Microsoft.Xrm.Sdk.Entity entity)
        {
            foreach (var k in entity.Attributes.Keys.ToArray())
            {
                var v = entity[k];
                if (v == null) entity.Attributes.Remove(k);
            }
        }
    }
}
