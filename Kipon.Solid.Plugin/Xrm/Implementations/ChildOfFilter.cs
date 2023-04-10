namespace Kipon.Xrm.Implementations
{
    using Attributes;
    using Microsoft.Xrm.Sdk;
    using Extensions.Sdk;
    using System.Linq;

    public class ChildOfFilter : IMethodCondition
    {
        public bool Execute(IfAttribute ifAttr, IPluginExecutionContext currentCtx)
        {
            if (ifAttr is Attributes.Filter.ChildOfAttribute coa)
            {
                if (currentCtx.Depth == 1)
                {
                    return false;
                }

                if (currentCtx.ParentContext == null)
                {
                    return false;
                }

                var ctx = currentCtx.ParentContext;

                Microsoft.Xrm.Sdk.EntityReference refId = null;
                if (coa.ReferenceAttributeName != null)
                {
                    var field = coa.ReferenceAttributeName.ToLower();
                    if (ctx.InputParameters.Contains("Target") && ctx.InputParameters["Target"] is Entity target && target.Attributes.ContainsKey(field)) 
                    {
                        refId = (Microsoft.Xrm.Sdk.EntityReference)target[field];
                    } else
                    {
                        refId = ctx.PreValueOf<Microsoft.Xrm.Sdk.EntityReference>(field);
                    }

                    if (refId == null)
                    {
                        return false;
                    }

                    return ctx.IsChildOf(coa.Message, coa.EntityLogicalName, refId.Id);
                }

                return ctx.IsChildOf(coa.Message, coa.EntityLogicalName, null); 
            }
            throw new InvalidPluginExecutionException($"Unexpected type of ifAttr { ifAttr.GetType().FullName }, expected ChildOfAttribute");
        }
    }
}
