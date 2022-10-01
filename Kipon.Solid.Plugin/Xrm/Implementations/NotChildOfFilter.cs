namespace Kipon.Xrm.Implementations
{
    using Attributes;
    using Microsoft.Xrm.Sdk;
    using Extensions.Sdk;

    public class NotChildOfFilter : IMethodCondition
    {
        public bool Execute(IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.Filter.NotChildOfAttribute coa)
            {
                Microsoft.Xrm.Sdk.EntityReference refId = null;
                if (coa.ReferenceAttributeName != null)
                {
                    var field = coa.ReferenceAttributeName.ToLower();
                    if (ctx.InputParameters.Contains("Target") && ctx.InputParameters["Target"] is Entity target && target.Attributes.ContainsKey(field))
                    {
                        refId = (Microsoft.Xrm.Sdk.EntityReference)target[field];
                    }
                    else
                    {
                        refId = ctx.PreValueOf<Microsoft.Xrm.Sdk.EntityReference>(field);
                    }

                    if (refId == null)
                    {
                        return true;
                    }

                    return !ctx.IsChildOf(coa.Message, coa.EntityLogicalName, refId.Id);
                }

                return !ctx.IsChildOf(coa.Message, coa.EntityLogicalName, null);
            }

            throw new InvalidPluginExecutionException($"Unexpected type of ifAttr { ifAttr.GetType().FullName }, expected NotChildOfAttribute");
        }
    }
}
