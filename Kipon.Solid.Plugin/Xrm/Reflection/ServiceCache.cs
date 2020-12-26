namespace Kipon.Xrm.Reflection
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    public class ServiceCache: System.IDisposable
    {
        private readonly Dictionary<string, object> services = new Dictionary<string, object>();
        private readonly object locks = new object();

        private Guid systemuserid;

        private IPluginExecutionContext pluginExecutionContext;
        private IOrganizationServiceFactory organizationServiceFactory;

        public ServiceCache(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService, IPluginContext pluginContext)
        {
            this.pluginExecutionContext = pluginExecutionContext;
            this.organizationServiceFactory = organizationServiceFactory;
            this.services.Add(typeof(IPluginExecutionContext).FullName, pluginExecutionContext);
            this.services.Add(typeof(IOrganizationServiceFactory).FullName, organizationServiceFactory);
            this.services.Add(typeof(ITracingService).FullName, traceService);

            if (pluginContext != null)
            {
                this.services.Add(typeof(IPluginContext).FullName, pluginContext);
            } else
            {
                var type = (CrmEventType)Enum.Parse(typeof(CrmEventType), pluginExecutionContext.MessageName);
                var pContext = new Services.PluginContext(null, null, pluginExecutionContext, type, pluginExecutionContext.UserId);
                this.services.Add(typeof(IPluginContext).FullName, pContext);
            }
            this.systemuserid = pluginExecutionContext.UserId;
        }

        public object Resolve(TypeCache type)
        {
            return this.Resolve(type, null);
        }

        public object Resolve(TypeCache type, Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            try
            {
                if (services.ContainsKey(type.ObjectInstanceKey))
                {
                    return services[type.ObjectInstanceKey];
                }

                lock (locks)
                {
                    if (services.ContainsKey(type.ObjectInstanceKey))
                    {
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.FromType == typeof(Guid) && type.Name != null && type.Name.ToLower() == nameof(this.pluginExecutionContext.PrimaryEntityId).ToLower())
                    {
                        return this.pluginExecutionContext.PrimaryEntityId;
                    }

                    if (type.FromType == typeof(string) && type.Name != null && type.Name.ToLower() == nameof(this.pluginExecutionContext.PrimaryEntityName).ToLower())
                    {
                        return this.pluginExecutionContext.PrimaryEntityName;
                    }

                    if (type.FromType == typeof(Microsoft.Xrm.Sdk.Query.QueryExpression))
                    {
                        if (pluginExecutionContext.InputParameters.Contains("Query"))
                        {
                            var query = pluginExecutionContext.InputParameters["Query"];
                            if (query is Microsoft.Xrm.Sdk.Query.QueryExpression qe)
                            {
                                // no cache by design - need to be resolved on all request.
                                return qe;
                            }

                            if (query is Microsoft.Xrm.Sdk.Query.FetchExpression fe)
                            {
                                var resp = (Microsoft.Crm.Sdk.Messages.FetchXmlToQueryExpressionResponse)orgService.Execute(new Microsoft.Crm.Sdk.Messages.FetchXmlToQueryExpressionRequest
                                {
                                    FetchXml = fe.Query
                                });
                                return resp.Query;
                            }

                            throw new Exceptions.UnresolvedQueryParameter(type.Name);
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("QueryExpression can only be requested for RetrieveMultiple requests");
                        }
                    }

                    if (type.IsTarget && !type.IsReference)
                    {
                        var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.InputParameters["Target"];
                        services[type.ObjectInstanceKey] = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(entity);
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.IsPreimage)
                    {
                        var imgName = PluginMethod.ImageSuffixFor(1, pluginExecutionContext.Stage, pluginExecutionContext.Mode == 1);
                        var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PreEntityImages[imgName];
                        services[type.ObjectInstanceKey] = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(entity);
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.IsPostimage)
                    {
                        var imgName = PluginMethod.ImageSuffixFor(2, pluginExecutionContext.Stage, pluginExecutionContext.Mode == 1);
                        var entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PostEntityImages[$"postimage{imgName}"];
                        services[type.ObjectInstanceKey] = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(entity);
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.IsMergedimage)
                    {
                        var target = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.InputParameters["Target"];
                        var merged = new Microsoft.Xrm.Sdk.Entity();
                        merged.Id = target.Id;
                        merged.LogicalName = target.LogicalName;

                        if (pluginExecutionContext.MessageName == "Create")
                        {
                            merged = target; 
                        } else
                        {
                            var imgName = PluginMethod.ImageSuffixFor(1, pluginExecutionContext.Stage, pluginExecutionContext.Mode == 1);
                            var pre = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.PreEntityImages[imgName];

                            foreach (var attr in pre.Attributes.Keys)
                            {
                                merged[attr] = pre[attr];
                            }

                            foreach (var attr in target.Attributes.Keys)
                            {
                                merged[attr] = target[attr];
                            }
                        }

                        services[type.ObjectInstanceKey] = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(merged);
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.IsReference)
                    {
                        var target = (Microsoft.Xrm.Sdk.EntityReference)pluginExecutionContext.InputParameters["Target"];
                        if (type.Constructor != null)
                        {
                            services[type.ObjectInstanceKey] = type.Constructor.Invoke(new object[] { target });
                        }
                        else
                        {
                            services[type.ObjectInstanceKey] = target;
                        }
                        return services[type.ObjectInstanceKey];
                    }

                    if (type.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                    {
                        return this.GetOrganizationService(type.RequireAdminService);
                    }

                    if (type.IsQuery)
                    {
                        var uow = this.GetIUnitOfWork(type.RequireAdminService);
                        var queryProperty = type.RepositoryProperty;
                        var repository = queryProperty.GetValue(uow, new object[0]);
                        var queryMethod = type.QueryMethod;
                        return queryMethod.Invoke(repository, new object[0]);
                    }

                    if (type.FromType == typeof(Guid))
                    {
                        if (type.Name.ToLower() == "id")
                        {
                            return pluginExecutionContext.PrimaryEntityId;
                        }

                        if (type.Name.ToLower() == "listid")
                        {
                            return pluginExecutionContext.InputParameters["ListId"];
                        }

                        if (type.Name.ToLower() == "entityid")
                        {
                            return pluginExecutionContext.InputParameters["EntityId"];
                        }

                        throw new Exceptions.UnresolveableParameterException(type.FromType, type.Name);
                    }

                    return this.CreateServiceInstance(type);
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                var typeName = (type?.FromType?.FullName) ?? "type was null";
                var key = type?.ObjectInstanceKey ?? "object instance key as null";
                throw new InvalidPluginExecutionException($"Unable to resolve service {typeName}, {key} or one of its dependencies.");
            }
        }

        private List<string> resolving = new List<string>();

        private IUnitOfWork GetIUnitOfWork(bool admin)
        {
            TypeCache tc = TypeCache.ForUow(admin);
            return (IUnitOfWork)this.CreateServiceInstance(tc);
        }

        private Microsoft.Xrm.Sdk.IOrganizationService GetOrganizationService(bool admin)
        {
            var objectInstanceKey = typeof(Microsoft.Xrm.Sdk.IOrganizationService).FullName;
            if (admin)
            {
                objectInstanceKey += ":admin";
            }
            if (services.ContainsKey(objectInstanceKey))
            {
                return (Microsoft.Xrm.Sdk.IOrganizationService)services[objectInstanceKey];
            }
            if (admin)
            {
                services[objectInstanceKey] = this.organizationServiceFactory.CreateOrganizationService(null);
            }
            else
            {
                services[objectInstanceKey] = this.organizationServiceFactory.CreateOrganizationService(this.systemuserid);
            }

            return (Microsoft.Xrm.Sdk.IOrganizationService)services[objectInstanceKey];
        }

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

        public void OnStepFinalize()
        {
            if (this.services != null)
            {
                foreach (var s in this.services.Values)
                {
                    var asdispos = s as IService;
                    if (asdispos != null)
                    {
                        asdispos.OnStepFinalized();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (this.services != null)
            {
                foreach (var s in this.services.Values)
                {
                    var asdispos = s as System.IDisposable;
                    if (asdispos != null)
                    {
                        asdispos.Dispose();
                    }
                }
                this.services.Clear();
            }
        }
    }
}
