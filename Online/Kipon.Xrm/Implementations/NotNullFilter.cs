namespace Kipon.Xrm.Implementations
{
    using Microsoft.Xrm.Sdk;
    using System.Linq;
    public class NotNullFilter : IMethodCondition
    {
        public bool Execute(Attributes.IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.Filter.NotNullAttribute nn)
            {
                if (nn.Attributes != null && nn.Attributes.Length > 0)
                {
                    var target = ctx.InputParameters["Target"] as Entity;
                    if (target != null)
                    {
                        foreach (var att in nn.Attributes.Select(r => r.ToLower()))
                        {
                            if (target.Attributes.ContainsKey(att) && target[att] != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
