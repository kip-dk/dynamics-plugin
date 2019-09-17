using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm.DI.Reflection
{
    public class PluginMethodCache
    {
        public static readonly Dictionary<string, PluginMethodCache> cache = new Dictionary<string, PluginMethodCache>();

        public static PluginMethodCache ForPlugin(Type type, int stage, string message, string primaryEntityName, bool isAsync)
        {
            var key = type.FullName + "|" + stage + "|" + message + "|" + (primaryEntityName ?? "null" + "|" + isAsync.ToString());

            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var lookFor = $"On{stage.ToStage()}{message}{(isAsync?"Async":"")}";

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var method in methods)
            {
                if (method.Name == lookFor)
                {
                }
            }

            return null;
        }



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
