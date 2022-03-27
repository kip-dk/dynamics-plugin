using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Tools.Models
{
    public class WebResourceMerge
    {

        private const string INCLUDE_REF = "//#include";

        private string path = null;
        public WebResourceMerge()
        {
            this.path = @".\";
        }


        public bool IsChanged(string file, System.DateTime? lastChange)
        {
            if (lastChange == null)
            {
                return true;
            }

            return this.DoIsChanged(file, lastChange.Value, false);
        }

        private bool DoIsChanged(string file, System.DateTime lastChange, bool nested = false)
        {
            if (nested)
            {
                file = file.TsToJsFileName();
            }

            var alllines = (from l in System.IO.File.ReadLines(file) select l).ToArray();
            if ((new FileInfo(file).LastWriteTimeUtc > lastChange))
            {
                return true;
            }

            var includes = (from l in alllines where l.StartsWith(INCLUDE_REF) select l.Split(' ')[1]).ToArray();

            foreach (var include in includes)
            {
                var name = path + include;
                var changed = DoIsChanged(name, lastChange, true);
                if (changed)
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> added = new List<string>();

        public byte[] GetMergedFileContent(string file, Entities.WebResourceTypeEnum type)
        {
            this.added = new List<string>();
            if (type == Entities.WebResourceTypeEnum.Jscript)
            {
                var sb = new StringBuilder();
                var lines = System.IO.File.ReadAllLines(file);

                if (lines.SpaceOnly())
                {
                    return null;
                }

                AddLines(sb, lines);
                return Encoding.UTF8.GetBytes(sb.ToString());
            }
            else
            {
                return System.IO.File.ReadAllBytes(file);
            }
        }

        private void AddLines(StringBuilder sb, string[] lines)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith(INCLUDE_REF))
                {
                    var file = line.Split(' ')[1].Trim().TsToJsFileName();

                    if (this.added.Contains(file.ToLower()))
                    {
                        continue;
                    }

                    sb.Append("// Start include " + file + "\n");
                    var subFileLines = System.IO.File.ReadAllLines(path + file);
                    AddLines(sb, subFileLines);
                    sb.Append("// End include " + file + "\n");

                    this.added.Add(file.ToLower());
                }
                else
                {
                    if (line.StartsWith("/// <reference path"))
                    {
                        continue;
                    }

                    if (line.StartsWith("//# sourceMappingURL="))
                    {
                        continue;
                    }
                    sb.Append(line + "\n");
                }
            }
        }
    }

    public static class WebResourceMergeLocalExtensions
    {
        public static string TsToJsFileName(this string file)
        {
            if (file.EndsWith(".ts"))
            {
                return file.Replace(".ts", ".js");
            }

            if (!file.EndsWith(".js"))
            {
                return $"{file}.js";
            }

            return file;
        }
    }
}
