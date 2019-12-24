using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Extensions.Strings
{
    public static class StringsExtensions
    {
        public static string GetParameter(this string url, string name, char sep = ';')
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            var spl = url.Split(sep).Select(r => r.Replace(" ", ""));
            var match = (from s in spl where s.ToLower().StartsWith(name.ToLower() + "=") select s).FirstOrDefault();
            if (!string.IsNullOrEmpty(match))
            {
                var index = match.IndexOf("=");
                return match.Substring(index + 1).Trim();
            }
            return null;
        }
    }
}
