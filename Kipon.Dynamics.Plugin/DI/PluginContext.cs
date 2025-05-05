using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Dynamics.Plugin.Attributes;
using Microsoft.Xrm.Sdk;

namespace Kipon.Dynamics.Plugin.DI
{
    public class PluginContext : IPluginContext
    {
        internal static DI.ServiceFactory serviceFactory = DI.ServiceFactory.Instance;
        private System.Collections.Generic.Dictionary<Type, object> di;


        private IPluginExecutionContext PluginExecutionContext;
        private ITracingService TracingService;
        private IOrganizationService OrganizationService;

        internal PluginContext(string unsecureConfig, string secureConfig, IPluginExecutionContext pluginExecutionContext, ITracingService tracingService, IOrganizationService organizationService, CrmEventType eventType, Guid userid, System.Collections.Generic.Dictionary<Type, object> _di, Type callingType)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
            this.PluginExecutionContext = pluginExecutionContext;
            this.TracingService = tracingService;
            this.OrganizationService = organizationService;
            this.EventType = eventType;
            this.UserId = userid;
            this.di = _di;
            serviceFactory.RegisterExternalExports(callingType);
        }

        public string UnsecureConfig { get; private set; }

        public string SecureConfig { get; private set; }

        public CrmEventType EventType { get; private set; }

        public Guid UserId { get; private set; }

        public EntityReference TargetReference
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("Target"))
                {
                    return (EntityReference)PluginExecutionContext.InputParameters["Target"];
                }

                return null;
            }
        }

        public EntityReferenceCollection Associations
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("RelatedEntities"))
                {
                    var dy = (Microsoft.Xrm.Sdk.EntityReferenceCollection)PluginExecutionContext.InputParameters["RelatedEntities"];
                    return dy;
                }

                return null;
            }
        }

        public Entity Preimage
        {
            get
            {
                return PluginExecutionContext.PreEntityImages["preimage"];

            }
        }

        public Entity Target
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("Target"))
                {
                    var obj = PluginExecutionContext.InputParameters["Target"];
                    if (obj is Microsoft.Xrm.Sdk.Entity)
                    {
                        return (Microsoft.Xrm.Sdk.Entity)obj;
                    }

                    if (obj is Microsoft.Xrm.Sdk.EntityReference)
                    {
                        var result = new Entity { Id = PluginExecutionContext.PrimaryEntityId, LogicalName = PluginExecutionContext.PrimaryEntityName };
                        if (PluginExecutionContext.InputParameters.Contains("State"))
                        {
                            result["statecode"] = PluginExecutionContext.InputParameters["State"] as OptionSetValue;
                        }

                        if (PluginExecutionContext.InputParameters.Contains("Status"))
                        {
                            result["statuscode"] = PluginExecutionContext.InputParameters["Status"] as OptionSetValue;
                        }
                        return result;
                    }
                }
                else
                {
                    if (PluginExecutionContext.InputParameters.Contains("EntityMoniker"))
                    {
                        var obj = PluginExecutionContext.InputParameters["EntityMoniker"] as EntityReference;
                        var result = new Entity { Id = PluginExecutionContext.PrimaryEntityId, LogicalName = PluginExecutionContext.PrimaryEntityName };
                        if (PluginExecutionContext.InputParameters.Contains("State"))
                        {
                            result["state"] = PluginExecutionContext.InputParameters["State"] as OptionSetValue;
                        }

                        if (PluginExecutionContext.InputParameters.Contains("Status"))
                        {
                            result["statuscode"] = PluginExecutionContext.InputParameters["Status"] as OptionSetValue;
                        }
                        return result;
                    }
                }
                return null;
            }
        }

        public bool AttributeChanged(params string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return false;
            }
            if (PluginExecutionContext.InputParameters.Contains("Target"))
            {
                var dy = PluginExecutionContext.InputParameters["Target"] as Microsoft.Xrm.Sdk.Entity;

                if (dy != null)
                {
                    foreach (var name in names)
                    {
                        if (dy.Attributes.Contains(name))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public Entity GetFullImage()
        {
            if (this.EventType == CrmEventType.Create)
            {
                return this.Target;
            }

            Entity entity = null;

            if (this.EventType == CrmEventType.Delete)
            {
                entity = new Entity();
                entity.Id = this.TargetReference.Id;
                entity.LogicalName = this.TargetReference.LogicalName;
            }
            else
            {
                entity = this.Target;
            }

            var full = new Entity();
            full.Id = entity.Id;
            full.LogicalName = entity.LogicalName;

            if (PluginExecutionContext.PreEntityImages.ContainsKey("preimage"))
            {
                var preImage = PluginExecutionContext.PreEntityImages["preimage"];

                foreach (var attribName in preImage.Attributes.Keys)
                {
                    full[attribName] = preImage[attribName];
                }
            }

            foreach (var attribName in entity.Attributes.Keys)
            {
                full[attribName] = entity[attribName];
            }
            return full;
        }

        public T GetService<T>() where T: class
        {
            return serviceFactory.GetService<T>(this.di);
        }
    }
}
