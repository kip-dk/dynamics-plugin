﻿using System;
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


            sb.AppendLine("/*");
            sb.AppendLine($" * Kipon.Xrm.Tools version {Kipon.Xrm.Tools.Version.No}, Kipon.Xrm for SOLID plugins. © Kipon ApS, 2019, 2020.");
            sb.AppendLine($" * ");
            sb.AppendLine($" * This code was auto-generated by accumulating the Kipon.Xrm functionality of Kipon.Solid.Plugin namespace into");
            sb.AppendLine($" * a single distributable source.");
            sb.AppendLine($" *");
            sb.AppendLine($" * It is against the license terms to change the namespace of any classes in this source.");
            sb.AppendLine($" *");
            sb.AppendLine($" * Any use of this code is on you own risk. Kipon ApS does not take any responsibility on issues caused or related to the use of this code.");
            sb.AppendLine(" */");

            foreach (var fil in files)
            {
                var txt = System.IO.File.ReadAllText(fil);
                sb.AppendLine($"#region source: {fil}");
                sb.AppendLine(txt);
                sb.AppendLine($"#endregion");
            }

            var result = sb.ToString();
            if (!string.IsNullOrEmpty(newNamespace))
            {
                result = result.Replace("namespace Kipon.Xrm", "namespace " + newNamespace);

            }
            System.IO.File.WriteAllText($@"{destinationPath}\Kipon.Xrm.cs", result);
        }
    }
}
