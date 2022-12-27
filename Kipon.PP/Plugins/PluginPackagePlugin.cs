using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PP.Plugins
{
    public class PluginPackagePlugin : Kipon.Xrm.BasePlugin
    {
        public void OnValidateCreate(Kipon.PP.Entities.PluginPackage target, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            if (!string.IsNullOrEmpty(target.Content))
            {
                var content = System.Convert.FromBase64String(target.Content);
                traceService.Trace(System.Text.Encoding.UTF8.GetString(content));
            } else
            {
                traceService.Trace("Content is null or empty");
            }
        }
    }
}
