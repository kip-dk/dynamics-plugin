using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeCopy
{
    public class CopyXrmCode
    {
        public static void Copy(string sourcePath, string destinationPath, string newNamespace = null)
        {
            var sb = new StringBuilder();

            var files = System.IO.Directory.GetFiles(sourcePath, "*.cs", System.IO.SearchOption.AllDirectories);

            foreach (var fil in files)
            {
                var txt = System.IO.File.ReadAllText(fil);
                sb.AppendLine($"/* START: {fil} */");
                sb.AppendLine(txt);
                sb.AppendLine($"/* END  : {fil} */");
            }

            var result = sb.ToString();
            if (!string.IsNullOrEmpty(newNamespace))
            {
                if (!newNamespace.EndsWith("."))
                {
                    newNamespace += ".";
                }
                result = result.Replace("Kipon.Xrm.", newNamespace);

                System.IO.File.WriteAllText($@"{destinationPath}\Kipon.Xrm.cs", result);
            }
        }
    }
}
