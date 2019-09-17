using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Extensions.Strings
{
    public static class StringMethods
    {
        public static string FirstToUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.First().ToString().ToUpper() + value.Substring(1).ToLower();
        }
    }
}
