using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter
{
    internal class SharedCustomizeCodeDomService
    {
        private System.IO.StreamWriter writer;
        internal SharedCustomizeCodeDomService(System.IO.StreamWriter writer)
        {
            this.writer = writer;
        }

        internal void GlobalOptionSets(IEnumerable<Model.OptionSet> optionsets)
        {
            if (optionsets != null)
            {
                foreach (var optionset in optionsets)
                {
                    writer.WriteLine($"\tpublic enum {optionset.Name}");
                    writer.WriteLine("\t{");

                    var count = optionsets.Count();

                    var current = 0;
                    foreach (var value in optionset.Values)
                    {
                        current++;
                        writer.WriteLine($"\t\t{value.Name} = {value.Value} {(current == count ? "" : ",")}");
                    }
                    writer.WriteLine("\t}");
                }
            }
        }
    }
}
