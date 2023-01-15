using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.SpecialService
{
    public class SpecialServicePlugin : Kipon.Xrm.BasePlugin
    {
        public void OnValidateCreate(Entities.kipon_multitest target, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
        }

        public void OnPreCreate(Entities.kipon_multitest target, ServiceAPI.ISomeExternalService sa)
        {
            var baseName = target.kipon_Name ?? string.Empty;
            target.kipon_Name = $"{ baseName } {sa.GetNameOf(target.LogicalName, target.Id) }";
        }
    }
}
