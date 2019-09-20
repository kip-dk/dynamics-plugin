using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI.Reflection
{
    public class PluginMethodCache
    {
        public static readonly Dictionary<string, PluginMethodCache[]> cache = new Dictionary<string, PluginMethodCache[]>();

        private PluginMethodCache()
        {
        }

        public static PluginMethodCache[] ForPlugin(Type type, int stage, string message, string primaryEntityName, bool isAsync)
        {
            var key = type.FullName + "|" + stage + "|" + message + "|" + primaryEntityName + "|" + isAsync.ToString();

            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var lookFor = $"On{stage.ToStage()}{message}{(isAsync?"Async":"")}";

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var stepStage = stage == 40 && isAsync ? 41 : stage;

            List<PluginMethodCache> results = new List<PluginMethodCache>();

            foreach (var method in methods)
            {
                var cas = method.GetCustomAttributes(typeof(Attributes.StepAttribute), false);
                var found = false;
                foreach (Attributes.StepAttribute ca in cas)
                {
                    if ((int)ca.Stage == stepStage && ca.Message.ToString() == message && ca.PrimaryEntityName == primaryEntityName && ca.IsAsync == isAsync)
                    {
                        var next = CreateFrom(method);
                        results.Add(next);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                if (method.Name == lookFor)
                {
                    var next = CreateFrom(method);

                }
            }

            if (results.Count == 0)
            {
                throw new Exceptions.UnresolvablePluginMethodException(type);
            }

            cache[key] = results.ToArray();
            return cache[key];
        }

        private static PluginMethodCache CreateFrom(System.Reflection.MethodInfo method)
        {
            var result = new PluginMethodCache();
            var parameters = method.GetParameters().DefaultIfEmpty().ToArray();

            result.Parameters = new TypeCache[parameters.Length];
            var ix = 0;
            foreach (var parameter in parameters)
            {
                result.Parameters[ix] = TypeCache.ForParameter(parameter);
                ix++;
            }
            return result;
        }


        public TypeCache[] Parameters { get; private set; }

    }

    internal static class PluginMethodCacheLocalExtensions
    {
        public static string ToStage(this int value)
        {
            switch(value)
            {
                case 10: return "Validate";
                case 20: return "Pre";
                case 40: return "Post";
                default: throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"Unknown state {value}");
            }
        }
    }
}
