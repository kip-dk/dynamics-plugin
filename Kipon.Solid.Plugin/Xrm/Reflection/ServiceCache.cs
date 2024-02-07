namespace Kipon.Xrm.Reflection
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Collections.Generic;
    public class ServiceCache: System.IDisposable
    {
        private readonly Dictionary<string, object> services = new Dictionary<string, object>();
        private readonly object locks = new object();

        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }

        private Guid systemuserid;

        private readonly IPluginExecutionContext pluginExecutionContext;
        private readonly IOrganizationServiceFactory organizationServiceFactory;

        public ServiceCache(IPluginExecutionContext pluginExecutionContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService, IPluginContext pluginContext, string config, string secureConfig)
        {
            this.UnsecureConfig = config;
            this.SecureConfig = secureConfig;
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

        public ServiceCache(System.Activities.CodeActivityContext codeActivityContext, IWorkflowContext workflowContext, IOrganizationServiceFactory organizationServiceFactory, ITracingService traceService)
        {
            this.organizationServiceFactory = organizationServiceFactory;
            this.services.Add(typeof(IOrganizationServiceFactory).FullName, organizationServiceFactory);
            this.services.Add(typeof(ITracingService).FullName, traceService);

            this.pluginExecutionContext = new Xrm.Implementations.WorkflowPluginExecutionContext(workflowContext);
            this.services.Add(typeof(IPluginExecutionContext).FullName, this.pluginExecutionContext);


            var pContext = new Services.PluginContext(null, null, pluginExecutionContext, CrmEventType.Other, workflowContext.UserId);
            this.services.Add(typeof(IPluginContext).FullName, pContext);

            this.services.Add(typeof(System.Activities.CodeActivityContext).FullName, codeActivityContext);
            this.services.Add(typeof(IWorkflowContext).FullName, workflowContext);

            this.systemuserid = workflowContext.UserId;
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

                    if (type.FromType == typeof(string) && type.Name != null && type.Name.ToLower() == nameof(BasePlugin.UnsecureConfig).ToLower()) 
                    {
                        return this.UnsecureConfig;
                    }

                    if (type.FromType == typeof(string) && type.Name != null && type.Name.ToLower() == nameof(BasePlugin.SecureConfig).ToLower())
                    {
                        return this.SecureConfig;
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
                        var merged = new Microsoft.Xrm.Sdk.Entity();

                        var target = pluginExecutionContext.InputParameters["Target"] as Microsoft.Xrm.Sdk.Entity;

                        if (target != null)
                        {
                            merged.Id = target.Id;
                            merged.LogicalName = target.LogicalName;
                        }
                        else
                        {
                            var er = pluginExecutionContext.InputParameters["Target"] as Microsoft.Xrm.Sdk.EntityReference;
                            merged.Id = er.Id;
                            merged.LogicalName = er.LogicalName;
                        }

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

                            if (target != null)
                            {
                                foreach (var attr in target.Attributes.Keys)
                                {
                                    merged[attr] = target[attr];
                                }
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

                    if (type.IsAdminUnitOfWork)
                    {
                        return this.GetIUnitOfWork(true);
                    }

                    if (type.IsUnitOfWork)
                    {
                        return this.GetIUnitOfWork(false);
                    }

                    if (type.IsQuery)
                    {
                        var uow = this.GetIUnitOfWork(type.RequireAdminService);
                        var repositoryProperty = type.RepositoryProperty;
                        var repository = repositoryProperty.GetValue(uow, new object[0]);
                        var queryMethod = type.QueryMethod;
                        return queryMethod.Invoke(repository, new object[0]);
                    }

                    if (type.IsRepository)
                    {
                        var uow = this.GetIUnitOfWork(type.RequireAdminService);
                        var queryProperty = type.RepositoryProperty;
                        return queryProperty.GetValue(uow, new object[0]);
                    }

                    if (type.IsEntityCache)
                    {
                        var uow = this.GetIUnitOfWork(type.RequireAdminService);
                        var cacheProperty = uow.GetType().GetProperty("Cache");
                        return cacheProperty.GetValue(uow, new object[0]);
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

                    if (type.FromType == typeof(Microsoft.Xrm.Sdk.Relationship))
                    {
                        if (pluginExecutionContext.InputParameters.Contains("Relationship"))
                        {
                            return (Microsoft.Xrm.Sdk.Relationship)pluginExecutionContext.InputParameters["Relationship"];
                        }

                        throw new InvalidPluginExecutionException("Relationship is requested as input parameter but there is no such information in the payload");
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
                    if (argType.FromType == typeof(string) && argType.Name != null && argType.Name.ToLower() == nameof(BasePlugin.UnsecureConfig).ToLower())
                    {
                        args[ix] = this.UnsecureConfig;
                        ix++;
                        continue;
                    }

                    if (argType.FromType == typeof(string) && argType.Name != null && argType.Name.ToLower() == nameof(BasePlugin.SecureConfig).ToLower())
                    {
                        args[ix] = this.SecureConfig;
                        ix++;
                        continue;
                    }

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
                        var next = this.Resolve(argType);
                        if (next != null)
                        {
                            services[argType.ObjectInstanceKey] = next;
                            args[ix] = services[argType.ObjectInstanceKey];
                            ix++;
                            continue;
                        }
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
