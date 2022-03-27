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

        public static bool SpaceOnly(this string[] lines)
        {
            if (lines == null || lines.Length == 0)
            {
                return true;
            }

            foreach (var lin in lines)
            {
                var trim = lin.Trim();
                if (!string.IsNullOrEmpty(trim))
                {
                    return false;
                }
            }
            return true;
        }

        public static Entities.WebResourceTypeEnum ToFileType(this string filename)
        {
            var extension = filename.Split('.').LastOrDefault();

            if (!string.IsNullOrEmpty(extension))
            {
                switch (extension)
                {
                    case "htm":
                    case "html":
                        return Entities.WebResourceTypeEnum.Html;
                    case "css":
                        return Entities.WebResourceTypeEnum.Css;
                    case "js":
                        return Entities.WebResourceTypeEnum.Jscript;
                    case "xml":
                        return Entities.WebResourceTypeEnum.Xml;
                    case "png":
                        return Entities.WebResourceTypeEnum.Png;
                    case "jpg":
                    case "jpeg":
                        return Entities.WebResourceTypeEnum.Jpg;
                    case "gif":
                        return Entities.WebResourceTypeEnum.Gif;
                    case "xap":
                        return Entities.WebResourceTypeEnum.Xap;
                    case "xsl":
                        return Entities.WebResourceTypeEnum.Xsl;
                    case "ico":
                        return Entities.WebResourceTypeEnum.Ico;
                }
            }
            return Entities.WebResourceTypeEnum.Unknown;
        }

    }
}
