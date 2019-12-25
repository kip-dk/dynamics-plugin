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

        internal void EntityOptionsetProperties(Dictionary<string, Model.Entity> entities, Dictionary<string, Model.OptionSet> globalOptionSets, Dictionary<string,string> attrSchemaNameMap)
        {
            foreach (var logicalname in entities.Keys)
            {
                var entity = entities[logicalname];
                if (entity.Optionsets != null && entity.Optionsets.Length > 0)
                {
                    writer.WriteLine($"\tpublic partial class {logicalname}");
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
                        writer.WriteLine($"\t\t[Microsoft.Xrm.Sdk.AttributeLogicalName(\"{optionset.Logicalname}\")]");
                        var type = optionset.Id == null ? $"{entity.LogicalName}.{optionset.Name}Enum" : globalOptionSets[optionset.Id].Name;
                        var key = $"{entity.LogicalName}.{optionset.Logicalname}";
                        var schemaName = attrSchemaNameMap[key];

                        if (schemaName == optionset.Name)
                        {
                            throw new Exception($"Optionset on {logicalname} has defined property {optionset.Logicalname} with same name as the logical name. Tha will generate dublicate properties and is not allowed");
                        }


                        writer.WriteLine($"\t\tpublic {type}? {optionset.Name}");
                        writer.WriteLine("\t\t{");
                        writer.WriteLine("\t\t\tget");
                        writer.WriteLine("\t\t\t{");
                        writer.WriteLine($"\t\t\t\tif (this.{schemaName} != null)");
                        writer.WriteLine($"\t\t\t\t\treturn ({type})this.{schemaName}.Value;");
                        writer.WriteLine($"\t\t\treturn null;");
                        writer.WriteLine("\t\t\t}");

                        writer.WriteLine("\t\t}");
                        #endregion
                    }

                    writer.WriteLine("\t}");
                }
            }
        }
    }
}
