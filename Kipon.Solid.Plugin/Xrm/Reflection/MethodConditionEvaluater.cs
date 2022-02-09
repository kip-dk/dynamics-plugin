namespace Kipon.Xrm.Reflection
{
    using System;
    public class MethodConditionEvaluater
    {

        private static System.Collections.Generic.Dictionary<Type, IMethodCondition> conditions = new System.Collections.Generic.Dictionary<Type, IMethodCondition>();

        public static bool Evaluate(Attributes.IfAttribute attr, Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            var evaluator = MethodConditionEvaluater.GetCondition(attr.Type);
            return evaluator.Execute(attr, ctx);
        }

        private static IMethodCondition GetCondition(Type type)
        {
            if (conditions.TryGetValue(type, out IMethodCondition m))
            {
                return m;
            }

            lock (conditions)
            {
                if (conditions.TryGetValue(type, out IMethodCondition m2))
                {
                    return m2;
                }

                var result = Activator.CreateInstance(type);
                var x = result as IMethodCondition;
                if (x == null)
                {
                    throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"Type { type.FullName } does not implement Kipon.Xrm.IMethodCondition as expected");
                }

                conditions[type] = x;
                return x;
            }
        }
    }
}
