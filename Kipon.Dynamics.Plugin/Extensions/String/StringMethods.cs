using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Extensions.String
{
    public static class StringMethods
    {
        public static string UpperCaseWords(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var words = value.Split(new char[] { ' ', '\t' });
                words = (from w in words select w.UppercaseFirst()).ToArray();
                return string.Join(" ", words);
            }
            return null;
        }

        public static string UppercaseFirst(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
                if (value.Length == 1) return value.ToUpper();
                return value.Substring(0,1).ToUpper() + value.Substring(1).ToLower();
            }
            return value;
        }
    }
}
