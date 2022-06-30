namespace Kipon.Xrm.Implementations
{
    using System;
    using Microsoft.Xrm.Sdk;
    using System.Linq;
    using Xrm.Extensions.Sdk;
    public class ChildOfFilter : IMethodCondition
    {
        public bool Execute(Attributes.IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.Filter.ChildOfAttribute nn)
            {
                if (nn.ReferenceFieldName != null)
                {
                    var r = ((Entity)ctx.InputParameters["Target"]).GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>(nn.ReferenceFieldName.ToLower());

                    if (r == null) return false;

                    //return ctx.IsChildOf(nn.Message, r.LogicalName, r.Id);
                } else
                {
                    //return ctx.IsChildOf(nn.Message, nn.LogicalName);
                }
            }
            return false;
        }
    }
}
