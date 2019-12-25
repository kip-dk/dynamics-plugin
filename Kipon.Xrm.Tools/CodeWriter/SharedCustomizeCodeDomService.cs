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

        internal void EntityOptionsetProperties(Dictionary<string, Model.Entity> entities, Dictionary<string, Model.OptionSet> globalOptionSets, Dictionary<string,string> attrSchemaNameMap, bool standardSuppressed)
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
                        var type = optionset.Id == null ? $"{optionset.Name}Enum" : globalOptionSets[optionset.Id].Name;
                        var key = $"{entity.LogicalName}.{optionset.Logicalname}";
                        var schemaName = attrSchemaNameMap[key];

                        if (schemaName == optionset.Name && !standardSuppressed)
                        {
                            throw new Exception($"Optionset on {logicalname} has defined property {optionset.Logicalname} with same name as the schema name for the property. That will generate dublicate properties and is not allowed. You can supress standard optionset properties by adding the 'supress-mapped-standard-optionset-properties=\"true\" to the root filter element in the configuration file.");
                        }

                        if (!optionset.Multi)
                        {
                            writer.WriteLine($"\t\tpublic {type}? {optionset.Name}");
                            writer.WriteLine("\t\t{");

                            writer.WriteLine("\t\t\tget");
                            writer.WriteLine("\t\t\t{");

                            if (!standardSuppressed)
                            {
                                writer.WriteLine($"\t\t\t\tif (this.{schemaName} != null)");
                                writer.WriteLine($"\t\t\t\t\treturn ({type})this.{schemaName}.Value;");
                            }
                            else
                            {
                                writer.WriteLine($"\t\t\t\tMicrosoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(\"{optionset.Logicalname}\");");
                                writer.WriteLine($"\t\t\t\tif (optionSet != null)");
                                writer.WriteLine("\t\t\t\t{");
                                writer.WriteLine($"\t\t\t\t\treturn ({type})optionSet.Value;");
                                writer.WriteLine("\t\t\t\t}");
                            }

                            writer.WriteLine($"\t\t\t\treturn null;");
                            writer.WriteLine("\t\t\t}");

                            writer.WriteLine("\t\t\tset");
                            writer.WriteLine("\t\t\t{");

                            if (!standardSuppressed)
                            {
                                writer.WriteLine($"\t\t\t\tif (value != null)");
                                writer.WriteLine("\t\t\t\t{");
                                writer.WriteLine($"\t\t\t\t\tthis.{schemaName} = new Microsoft.Xrm.Sdk.OptionSetValue((int)value);");
                                writer.WriteLine($"\t\t\t\t\treturn;");
                                writer.WriteLine("\t\t\t\t}");
                                writer.WriteLine($"\t\t\t\tthis.{schemaName} = null;");
                            }
                            else
                            {
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{schemaName}\");");
                                writer.WriteLine($"\t\t\t\tif (value != null)");
                                writer.WriteLine("\t\t\t\t{");
                                writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value));");
                                writer.WriteLine($"\t\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");
                                writer.WriteLine($"\t\t\t\t\treturn;");
                                writer.WriteLine("\t\t\t\t}");
                                writer.WriteLine($"\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", null);");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");
                            }


                            writer.WriteLine("\t\t\t}");

                            writer.WriteLine("\t\t}");
                        }
                        else
                        {
                            writer.WriteLine($"\t\tpublic {type}[] {optionset.Name}");
                            writer.WriteLine("\t\t{");

                            writer.WriteLine("\t\t\tget");
                            writer.WriteLine("\t\t\t{");

                            writer.WriteLine($"\t\t\t\tMicrosoft.Xrm.Sdk.OptionSetValueCollection optionSetValues = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>(\"{optionset.Logicalname}\");");
                            writer.WriteLine($"\t\t\t\tif (optionSetValues != null && optionSetValues.Count > 0)");
                            writer.WriteLine("\t\t\t\t{");
                            writer.WriteLine($"\t\t\t\t\treturn (from v in optionSetValues select ({type})v.Value).ToArray();");
                            writer.WriteLine("\t\t\t\t}");
                            writer.WriteLine($"\t\t\t\treturn null;");
                            writer.WriteLine("\t\t\t}");


                            writer.WriteLine("\t\t\tset");
                            writer.WriteLine("\t\t\t{");
                            writer.WriteLine("\t\t\t}");

                            writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{schemaName}\");");
                            writer.WriteLine($"\t\t\t\tif (value != null && value.Count > 0");
                            writer.WriteLine("\t\t\t\t{");
                            writer.WriteLine("\t\t\t\t\tvar result = new Microsoft.Xrm.Sdk.OptionSetValueCollection()");
                            writer.WriteLine($"\t\t\t\tforeach (var v in value) result.Add(new Microsoft.Xrm.Sdk.OptionSetValue(int)value);");
                            writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", result);");
                            writer.WriteLine($"\t\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");
                            writer.WriteLine($"\t\t\t\t\treturn;");
                            writer.WriteLine("\t\t\t\t}");
                            writer.WriteLine($"\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", null);");
                            writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");


                            writer.WriteLine("\t\t}");
                        }
                        #endregion
                    }

                    writer.WriteLine("\t}");
                }
            }
        }
    }
}
