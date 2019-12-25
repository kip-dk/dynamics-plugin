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

                    var count = optionset.Values.Length;

                    var current = 0;
                    foreach (var value in optionset.Values)
                    {
                        current++;
                        writer.WriteLine($"\t\t{value.Name} = {value.Value}{(current == count ? "" : ",")}");
                    }
                    writer.WriteLine("\t}");
                }
            }
        }

        internal void EntityOptionsetProperties(Model.Entity[] Entities, Dictionary<string, Model.OptionSet> globalOptionSets)
        {
            foreach (var entity in Entities)
            {
                if (entity.Optionsets != null && entity.Optionsets.Length > 0)
                {
                    writer.WriteLine($"\tpublic partial class {entity.LogicalName}");
                    writer.WriteLine("\t{");

                    foreach (var optionset in entity.Optionsets)
                    {
                        #region generate local optionset
                        if (optionset.Id == null)
                        {
                            writer.WriteLine($"\t\tpublic enum {optionset.Name}Enum");
                            writer.WriteLine("\t\t{");
                            var count = optionset.Values.Length;
                            var current = 0;
                            foreach (var value in optionset.Values)
                            {
                                writer.WriteLine($"\t\t\t{value.Name} = {value.Value}{(current == count ? "" : ",")}");
                                current++;
                            }
                            writer.WriteLine("\t\t}");
                        }
                        #endregion

                        #region generate property
                        writer.WriteLine($"\t[Microsoft.Xrm.Sdk.AttributeLogicalName(\"{optionset.Logicalname}\")]");
                        var type = optionset.Id == null ? $"{entity.LogicalName}.{optionset.Name}Enum" : globalOptionSets[optionset.Id].Name;
                        writer.WriteLine($"\tpublic enum {type}? {optionset.Name}");
                        writer.WriteLine("\t{");
                        writer.WriteLine("\t\tget");
                        writer.WriteLine("\t\t{");
                        writer.WriteLine($"\t\t\tif (this.{optionset.Logicalname} != null)");
                        writer.WriteLine($"\t\t\t\treturn ({type})this.{optionset.Logicalname}.Value;");
                        writer.WriteLine($"\t\t\treturn null;");
                        writer.WriteLine("\t\t}");
                        writer.WriteLine("\t}");
                        #endregion
                    }
                    writer.WriteLine("\t}");
                }
            }
        }
    }
}
