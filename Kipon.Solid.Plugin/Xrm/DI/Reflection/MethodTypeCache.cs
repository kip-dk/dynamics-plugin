using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm.DI.Reflection
{
    public class MethodTypeCache
    {
        private static readonly Dictionary<System.Reflection.MethodInfo, MethodTypeCache> methodTypes = new Dictionary<System.Reflection.MethodInfo, MethodTypeCache>();


        public static MethodTypeCache ForMethod(System.Reflection.MethodInfo method)
        {
            if (methodTypes.ContainsKey(method))
            {
                return methodTypes[method];
            }

            var mt = new MethodTypeCache();
            mt.Privileged = method.GetCustomAttributes(typeof(Attributes.PrivilegedAttribute), false).Any();
            var parameters = method.GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                mt.Types = new Type[0];
            }
            else
            {
                mt.Types = new Type[parameters.Length];
                var ix = 0;
                foreach (var p in parameters)
                {
                    mt.Types[ix] = p.ParameterType;
                    ix++;
                }
            }
            methodTypes[method] = mt;
            return mt;
        }

        public bool Privileged { get; set; }
        public Type[] Types { get; set; }


    }
}
