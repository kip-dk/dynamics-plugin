using Kipon.Xrm.Extensions.QueryExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Virtual
{
    public class VirtualEntityPlugin : Kipon.Xrm.VirtualEntityPlugin
    {
        public Microsoft.Xrm.Sdk.Entity OnRetrieve(Guid primaryentityid, string primaryentityname)
        {
            return new Microsoft.Xrm.Sdk.Entity { LogicalName = primaryentityname, Id = primaryentityid };
        }

        public Microsoft.Xrm.Sdk.EntityCollection OnRetrieveMultiple(string primaryentityname, Microsoft.Xrm.Sdk.Query.QueryExpression query, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            query.Trace(traceService);

            var result = new Microsoft.Xrm.Sdk.EntityCollection();
            for (var i = 0; i < 10; i++)
            {
                result.Entities.Add(new Microsoft.Xrm.Sdk.Entity { LogicalName = primaryentityname, Id = Guid.NewGuid() });
            }
            return result;
        }
    }
}
