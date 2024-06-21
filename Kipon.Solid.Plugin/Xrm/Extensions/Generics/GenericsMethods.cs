
namespace Kipon.Xrm.Extensions.Generics
{
    using System.Collections.Generic;

    public static class GenericsMethods
    {
        public static object Safe(this Dictionary<string, object> index, string name)
        {
            if (index.TryGetValue(name, out object v)) 
            {
                return v;
            }
            return null;
        }
    }
}
