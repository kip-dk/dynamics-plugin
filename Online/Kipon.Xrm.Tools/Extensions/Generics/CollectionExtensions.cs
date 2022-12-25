using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Extensions.Generics
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T[]> Pages<T>(this IEnumerable<T> input, int pageSize)
        {
            var result = new List<T[]>();

            var arr = input.ToArray();

            var next = arr.Take(pageSize).ToArray();
            while (next.Length > 0)
            {
                result.Add(next);
                arr = arr.Skip(pageSize).ToArray();
                next = arr.Take(pageSize).ToArray();
            }
            return result;
        }
    }
}
