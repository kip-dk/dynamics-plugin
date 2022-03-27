namespace Kipon.Xrm.Reflection
{
    using System.Collections.Generic;
    public class ServiceConstructorCache
    {
        private readonly static Dictionary<System.Reflection.ConstructorInfo, TypeCache[]> cache = new Dictionary<System.Reflection.ConstructorInfo, TypeCache[]>();

        private static readonly object locks = new object();

        public static TypeCache[] ForConstructor(System.Reflection.ConstructorInfo constructor)
        {
            if (cache.ContainsKey(constructor))
            {
                return cache[constructor];
            }

            lock (locks)
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
                    result[ix] = TypeCache.ForParameter(par, null);
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
}
