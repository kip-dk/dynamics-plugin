using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI.Reflection
{
    public class ServiceConstructorCache
    {
        private readonly static Dictionary<System.Reflection.ConstructorInfo, TypeCache[]> cache = new Dictionary<System.Reflection.ConstructorInfo, TypeCache[]>();

        public static TypeCache[] ForConstructor(System.Reflection.ConstructorInfo constructor)
        {
            if (cache.ContainsKey(constructor))
            {
                return cache[constructor];
            }

            var parameters = constructor.GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                cache[constructor] = new TypeCache[0];
                return cache[constructor];
            }

            var result = new TypeCache[parameters.Length];

            var ix = 0;
            foreach (var par in parameters)
            {
                result[ix] = TypeCache.ForParameter(par);
                if (result[ix].RequirePluginContext)
                {
                    throw new Exceptions.InvalidConstructorServiceArgumentException(constructor, par);
                }
                ix++;
            }
            cache[constructor] = result;
            return result;
        }
    }
}
