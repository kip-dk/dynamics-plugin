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

        #endregion

        #region Methods
        public void Execute(IServiceProvider serviceProvider)
        {
            this.context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Guid? userId = null;

            if (!RunWithSystemPriviliges)
            {
                userId = context.UserId;
            }

            var service = serviceFactory.CreateOrganizationService(userId);

            var et = CrmEventType.Other;

            Enum.TryParse<CrmEventType>(context.MessageName, out et);
            var di = new System.Collections.Generic.Dictionary<Type, object>();

            try
            {
                var pc = new DI.PluginContext(this.UnsecureConfig, this.SecureConfig, context, tracingService, service, et, context.UserId, di);
                di.Add(typeof(IServiceProvider), serviceProvider);
                di.Add(typeof(IOrganizationServiceFactory), serviceFactory);
                di.Add(typeof(IPluginExecutionContext), context);
                di.Add(typeof(ITracingService), tracingService);
                di.Add(typeof(IOrganizationService), service);
                di.Add(typeof(DI.IPluginContext), pc);

                this.Execute(pc);
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
            finally
            {
                foreach (var s in di.Keys)
                {
                    try
                    {
                        var o = di[s] as IDisposable;
                        if (o != null)
                        {
                            o.Dispose();
                        }
                    } catch (Exception)
                    {
                        // if a cleanup fails, just continue with other cleanups.
                    }
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
        #endregion

        #region Abstract members

        protected abstract void Execute(DI.IPluginContext pluginContext);


        public virtual bool RunWithSystemPriviliges
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
