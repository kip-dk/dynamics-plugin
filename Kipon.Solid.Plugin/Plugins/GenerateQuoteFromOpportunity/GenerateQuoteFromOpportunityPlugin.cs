using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.GenerateQuoteFromOpportunity
{
    public class GenerateQuoteFromOpportunityPlugin : Kipon.Xrm.BasePlugin
    {
        /*
        public void OnPreGenerateQuoteFromOpportunity(Actions.IGenerateQuoteFromOpportunityRequest request, Microsoft.Xrm.Sdk.ITracingService trace, Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            trace.Trace($"Opp: { request.OpportunityId }");

            var parent = ctx.ParentContext;
            while (parent != null)
            {
                trace.Trace($"Parent: { parent.MessageName }: { parent.Mode }");
                parent = parent.ParentContext;
            }

            if (request.ColumnSet != null && request.ColumnSet.Columns != null && request.ColumnSet.Columns.Count > 0)
            foreach (var col in request.ColumnSet.Columns)
            {
                    trace.Trace($"Col: { col }");
            }
        }
        */
    }
}
