namespace Kipon.Xrm.Services
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class PluginContext : IPluginContext
    {
        private IPluginExecutionContext PluginExecutionContext;

        internal PluginContext(string unsecureConfig, string secureConfig, IPluginExecutionContext pluginExecutionContext, CrmEventType eventType, Guid userid)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
            this.PluginExecutionContext = pluginExecutionContext;
            this.EventType = eventType;
            this.UserId = userid;
        }

        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }
        public CrmEventType EventType { get; private set; }
        public Guid UserId { get; private set; }

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

                        if (!string.IsNullOrEmpty(name) && dy.Attributes.Contains(name.ToLower()))
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
    }
}
