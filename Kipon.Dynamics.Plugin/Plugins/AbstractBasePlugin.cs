using System;
using Microsoft.Xrm.Sdk;
using Kipon.Dynamics.Plugin.Attributes;

namespace Kipon.Dynamics.Plugin.Plugins
{
    [Solution(Name = "<Enter your solution name here>")]
    public abstract class AbstractBasePlugin : IPlugin
    {

        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }
        public Guid UserId { get; private set; }

        #region constructors
        public AbstractBasePlugin() : base()
        {
        }

        public AbstractBasePlugin(string unSecure, string secure) : base()
        {
            this.UnsecureConfig = unSecure;
            this.SecureConfig = secure;
        }
        #endregion

        #region Private members
        private IPluginExecutionContext context;
        private CrmEventType eventType;

        #endregion

        #region Methods
        public void Execute(IServiceProvider serviceProvider)
        {
            this.context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            this.UserId = context.UserId;

            Guid? userId = null;

            if (!RunWithSystemPriviliges)
            {
                userId = context.UserId;
            }

            var service = serviceFactory.CreateOrganizationService(userId);

            if (!Enum.TryParse<CrmEventType>(context.MessageName, out this.eventType))
            {
                this.eventType = CrmEventType.Other;
            }

            try
            {
                using (var uow = new Entities.CrmUnitOfWork(service, serviceFactory))
                {
                    Execute(uow, tracingService);
                }
            }
            catch (Microsoft.Xrm.Sdk.SaveChangesException ste)
            {
                var sb = new System.Text.StringBuilder();
                if (ste.Results != null)
                {
                    sb.Append("Deep: ");
                    foreach (SaveChangesResult res in ste.Results)
                    {
                        if (res.Error != null)
                        {
                            sb.Append(res.Error.Message);
                            if (res.Error.InnerException != null)
                            {
                                sb.Append(res.Error.InnerException.StackTrace);
                                sb.Append("/");
                            }
                        }
                    }
                }
                else
                {
                    sb.Append("Plain: " + ste.Message);
                }
                throw new InvalidPluginExecutionException("USC: " + sb.ToString() + " " + ste.StackTrace, ste);
            }
            catch (Exception ex)
            {
                if (ex is InvalidPluginExecutionException)
                {
                    // just throw it on, we already have a valid exception type
                    throw;
                }
                else
                {
                    // Unexpected error, we log and transform it into a valid exception
                    var mess = this.Log(tracingService, ex);
                    throw new InvalidPluginExecutionException(ex.GetType().FullName + " " + ex.Message, ex);
                }
            }
        }

        private string Log(ITracingService tracingService, Exception ex)
        {
            var soap = ex as System.Web.Services.Protocols.SoapException;
            string result = ex.Message;

            if (soap != null && soap.Detail != null)
            {
                tracingService.Trace("Message {0}, Stacktrace {1}", soap.Detail.InnerText, ex.StackTrace);
                result = soap.Detail.InnerText;
            }
            else
            {
                tracingService.Trace("Message {0}, Stacktrace {1}", ex.Message, ex.StackTrace);
                result = ex.Message;
            }

            var inner = ex.InnerException;
            if (inner != null)
            {
                this.Log(tracingService, inner);
            }
            return result;
        }

        /// <summary>
        /// Create a full image of the entity in pre-operations including all changes.
        /// </summary>
        /// <param name="entity">Input entity.</param>
        /// <param name="preImageName">Name of the pre-image.</param>
        /// <param name="context">Pluging exection context.</param>
        protected Entity GetFullImage()
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
            full.LogicalName = entity.LogicalName;

            if (context.PreEntityImages.ContainsKey("preimage"))
            {
                var preImage = context.PreEntityImages["preimage"];

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

        #endregion

        #region Abstract members

        public abstract void Execute(Entities.IUnitOfWork uow, ITracingService tracingService);

        public virtual bool RunWithSystemPriviliges
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Properties

        protected IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return context;
            }
        }

        protected bool AttributeChanged(params string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return false;
            }
            if (PluginExecutionContext.InputParameters.Contains("Target"))
            {
                var dy = context.InputParameters["Target"] as Microsoft.Xrm.Sdk.Entity;

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

        protected Microsoft.Xrm.Sdk.EntityReference TargetReference
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

        protected Microsoft.Xrm.Sdk.EntityReferenceCollection Associations
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

        protected Microsoft.Xrm.Sdk.Entity Preimage
        {
            get
            {
                return this.GetPreimage("preimage");
            }
        }

        protected Microsoft.Xrm.Sdk.Entity GetPreimage(string name)
        {
            return context.PreEntityImages[name];
        }

        protected Microsoft.Xrm.Sdk.Entity Target
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

        protected string Association
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("Relationship"))
                {
                    var rel = PluginExecutionContext.InputParameters["Relationship"] as Microsoft.Xrm.Sdk.Relationship;

                    if (rel != null)
                    {
                        return rel.SchemaName;
                    }
                }

                return null;
            }
        }

        public CrmEventType EventType
        {
            get
            {
                return eventType;
            }
        }

        #endregion
    }
}
