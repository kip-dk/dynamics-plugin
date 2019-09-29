namespace Kipon.Xrm.DI.Reflection
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using Kipon.Xrm.Extensions.Sdk;
    public class ServiceCache
    {
        private readonly Dictionary<string, object> services = new Dictionary<string, object>();

        private Guid systemuserid;

        private IPluginExecutionContext pluginExecutionContext;
        private IOrganizationServiceFactory organizationServiceFactory;

        public ServiceCache(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService)
        {
            this.pluginExecutionContext = pluginExecutionContext;
            this.organizationServiceFactory = organizationServiceFactory;
            this.services.Add(typeof(IPluginExecutionContext).FullName, pluginExecutionContext);
            this.services.Add(typeof(IOrganizationServiceFactory).FullName, organizationServiceFactory);
            this.services.Add(typeof(ITracingService).FullName, traceService);
            this.systemuserid = pluginExecutionContext.UserId;
        }

        public object Resolve(TypeCache type)
        {
            if (services.ContainsKey(type.ObjectInstanceKey))
            {
                return services[type.ObjectInstanceKey];
            }

            if (type.IsTarget)
            {
                var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.InputParameters["Target"];
                services[type.ObjectInstanceKey] = entity.ToEarlyBoundEntity();
                return services[type.ObjectInstanceKey];
            }

            if (type.IsPreimage)
            {
                var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PreEntityImages["preimage"];
                services[type.ObjectInstanceKey] = entity.ToEarlyBoundEntity();
                return services[type.ObjectInstanceKey];
            }

            if (type.IsPostimage)
            {
                var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PostEntityImages["postimage"];
                services[type.ObjectInstanceKey] = entity.ToEarlyBoundEntity();
                return services[type.ObjectInstanceKey];
            }

            if (type.IsMergedimage)
            {
                var target = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.InputParameters["Target"];
                var merged = new Microsoft.Xrm.Sdk.Entity();
                merged.Id = target.Id;
                merged.LogicalName = target.LogicalName;

                var pre = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PreEntityImages["preimage"];

                foreach (var attr in pre.Attributes.Keys)
                {
                    merged[attr] = pre[attr];
                }

                foreach (var attr in target.Attributes.Keys)
                {
                    merged[attr] = target[attr];
                }

                services[type.ObjectInstanceKey] = merged.ToEarlyBoundEntity();
                return services[type.ObjectInstanceKey];
            }

            if (type.IsReference)
            {
                var target = (Microsoft.Xrm.Sdk.EntityReference)pluginExecutionContext.InputParameters["Target"];
                if (type.Constructor != null)
                {
                    services[type.ObjectInstanceKey] = type.Constructor.Invoke(new object[] { target });
                } else
                {
                    services[type.ObjectInstanceKey] = target;
                }
                return services[type.ObjectInstanceKey];
            }

            if (type.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
            {
                services[type.ObjectInstanceKey] = this.organizationServiceFactory.CreateOrganizationService(this.systemuserid);
                return services[type.ObjectInstanceKey];
            }

            return this.CreateServiceInstance(type);
        }

        private List<string> resolving = new List<string>();

        private object CreateServiceInstance(TypeCache type)
        {
            if (resolving.Contains(type.ObjectInstanceKey))
            {
                throw new Exceptions.CircularDependencyException(string.Join(">", resolving.ToArray()));
            }
            try
            {
                resolving.Add(type.ObjectInstanceKey);
                var argTypes = ServiceConstructorCache.ForConstructor(type.Constructor);
                var args = new object[argTypes != null ? argTypes.Length : 0];

                if (args.Length == 0)
                {
                    services[type.ObjectInstanceKey] = type.Constructor.Invoke(args);
                    return services[type.ObjectInstanceKey];
                }

                var ix = 0;
                foreach (var argType in argTypes)
                {
                    if (argType.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                    {
                        if (type.RequireAdminService)
                        {
                            var key = typeof(Microsoft.Xrm.Sdk.IOrganizationService).FullName + ":admin";
                            if (services.ContainsKey(key))
                            {
                                args[ix] = services[key];
                                ix++;
                                continue;
                            }
                            else
                            {
                                services[key] = this.organizationServiceFactory.CreateOrganizationService(null);
                                args[ix] = services[key];
                                ix++;
                                continue;
                            }
                        }
                        else
                        {
                            var key = typeof(Microsoft.Xrm.Sdk.IOrganizationService).FullName;
                            if (services.ContainsKey(key))
                            {
                                args[ix] = services[key];
                                ix++;
                                continue;
                            }
                            else
                            {
                                services[key] = this.organizationServiceFactory.CreateOrganizationService(this.systemuserid);
                                args[ix] = services[key];
                                ix++;
                                continue;
                            }
                        }
                    }

                    if (services.ContainsKey(argType.ObjectInstanceKey))
                    {
                        args[ix] = services[argType.ObjectInstanceKey];
                        ix++;
                        continue;
                    }

                    {
                        services[argType.ObjectInstanceKey] = this.CreateServiceInstance(argType);
                        args[ix] = services[argType.ObjectInstanceKey];
                        ix++;
                        continue;
                    }
                }
                return type.Constructor.Invoke(args);
            }
            finally
            {
                resolving.Remove(type.ObjectInstanceKey);
            }
        }
    }
}
