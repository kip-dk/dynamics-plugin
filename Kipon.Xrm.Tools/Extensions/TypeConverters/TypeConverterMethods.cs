using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Extensions.TypeConverters
{
    public static class TypeConverterMethods
    {
        public static int ToInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (int.TryParse(value, out int v))
            {
                return v;
            }

            Console.WriteLine($"Cannot parse int: { value }");
            return 0;
        }
    }
}
