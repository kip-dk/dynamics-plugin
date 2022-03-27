
namespace Kipon.Xrm.Implementations
{
    using Microsoft.Xrm.Sdk;
    public class RelationshipMethodCondition : IMethodCondition
    {

        public bool Execute(Attributes.IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.RelationshipAttribute r)
            {
                var rel = ctx.InputParameters["Relationship"] as Microsoft.Xrm.Sdk.Relationship;
                if (rel != null)
                {
                    return rel.SchemaName == r.SchemaName;
                }
            }
            return false;
        }
    }
}
