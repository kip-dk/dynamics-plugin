using Microsoft.Crm.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Tools.CodeWriter
{
    internal class SharedCustomizeCodeDomService
    {
        private readonly System.IO.StreamWriter writer;
        private readonly IMetadataProviderService meta;

        internal SharedCustomizeCodeDomService(System.IO.StreamWriter writer, IMetadataProviderService meta)
        {
            this.writer = writer;
            this.meta = meta;
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
            var metadata = this.meta.LoadMetadata();
            foreach (var logicalname in entities.Keys)
            {
                var entity = entities[logicalname];

                var hasOpt = Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter.FILTER.OPTIONSETFIELDS.ContainsKey(logicalname.ToLower());
                var hasMul = Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter.FILTER.MULTIOPTIONSETFIELDS.ContainsKey(logicalname.ToLower());

                var entMeta = metadata.Entities.Where(r => r.LogicalName.ToLower() == logicalname.ToLower()).Single();
                var stateAtt = (Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata)entMeta.Attributes.Where(r => r.LogicalName.ToLower() == "statecode").SingleOrDefault();


                if (hasOpt || hasMul || stateAtt != null || (entity.Optionsets != null && entity.Optionsets.Length > 0))
                {
                    if (stateAtt != null)
                    {
                        #region generate std-statecode enum
                        writer.WriteLine("\t[System.Runtime.Serialization.DataContractAttribute()]");
                        writer.WriteLine($"\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"Kipon.Solid.Plugin\", \"{ Version.No }\")]");
                        writer.WriteLine($"\tpublic enum { logicalname }State");
                        writer.WriteLine("\t{");

                        foreach (var v in stateAtt.OptionSet.Options)
                        {
                            writer.WriteLine($"\t\t[System.Runtime.Serialization.EnumMemberAttribute()]");
                            writer.WriteLine($"\t\t{ v.Label.UserLocalizedLabel.Label.ToCSharpName() } = { v.Value.Value },");
                        }
                        writer.WriteLine("\t}");
                        #endregion

                    }

                    writer.WriteLine($"\tpublic partial class {logicalname}");
                    writer.WriteLine("\t{");

                    if (entity.Optionsets != null && entity.Optionsets.Length > 0)
                    {

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

                            if (!attrSchemaNameMap.ContainsKey(key))
                            {
                                throw new Exception($"No attribute mathing logical name {key} was found. Optionset {optionset.Name} on {entity.LogicalName} has a wrong logicalname.");
                            }
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
                                    writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{optionset.Name}\");");
                                    writer.WriteLine($"\t\t\t\tif (value != null)");
                                    writer.WriteLine("\t\t\t\t{");
                                    writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value));");
                                    writer.WriteLine($"\t\t\t\t\tthis.OnPropertyChanged(\"{optionset.Name}\");");
                                    writer.WriteLine($"\t\t\t\t\treturn;");
                                    writer.WriteLine("\t\t\t\t}");
                                    writer.WriteLine($"\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", null);");
                                    writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{optionset.Name}\");");
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

                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{schemaName}\");");
                                writer.WriteLine($"\t\t\t\tif (value != null && value.Length > 0)");
                                writer.WriteLine("\t\t\t\t{");
                                writer.WriteLine("\t\t\t\t\tvar result = new Microsoft.Xrm.Sdk.OptionSetValueCollection();");
                                writer.WriteLine($"\t\t\t\t\tforeach (var v in value) result.Add(new Microsoft.Xrm.Sdk.OptionSetValue((int)v));");
                                writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", result);");
                                writer.WriteLine($"\t\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");
                                writer.WriteLine($"\t\t\t\t\treturn;");
                                writer.WriteLine("\t\t\t\t}");
                                writer.WriteLine($"\t\t\t\tthis.SetAttributeValue(\"{optionset.Logicalname}\", null);");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{schemaName}\");");
                                writer.WriteLine("\t\t\t}");

                                writer.WriteLine("\t\t}");
                            }
                            #endregion

                        }
                    }

                    #region generate std-optionsets
                    {
                        if (Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter.FILTER.OPTIONSETFIELDS.TryGetValue(logicalname.ToLower(), out List<string> attrs))
                        {
                            foreach (var att in attrs)
                            {
                                writer.WriteLine($"\t\t[Microsoft.Xrm.Sdk.AttributeLogicalName(\"{att.ToLower()}\")]");
                                writer.WriteLine($"\t\tpublic Microsoft.Xrm.Sdk.OptionSetValue { att }");
                                writer.WriteLine("\t\t{");
                                writer.WriteLine($"\t\t\tget => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(\"{ att.ToLower() }\");");

                                writer.WriteLine("\t\t\tset");
                                writer.WriteLine("\t\t\t{");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{att}\");");
                                writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{ att.ToLower() }\", value);");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{att}\");");
                                writer.WriteLine("\t\t\t}");

                                writer.WriteLine("\t\t}");
                            }
                        }
                    }
                    #endregion

                    #region generate std-multi optionsets
                    {
                        if (Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter.FILTER.MULTIOPTIONSETFIELDS.TryGetValue(logicalname.ToLower(), out List<string> attrs))
                        {
                            foreach (var att in attrs)
                            {
                                writer.WriteLine($"\t\t[Microsoft.Xrm.Sdk.AttributeLogicalName(\"{att.ToLower()}\")]");
                                writer.WriteLine($"\t\tpublic Microsoft.Xrm.Sdk.OptionSetValueCollection { att }");
                                writer.WriteLine("\t\t{");
                                writer.WriteLine($"\t\t\tget => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>(\"{ att.ToLower() }\");");

                                writer.WriteLine("\t\t\tset");
                                writer.WriteLine("\t\t\t{");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{att}\");");
                                writer.WriteLine($"\t\t\t\t\tthis.SetAttributeValue(\"{ att.ToLower() }\", value);");
                                writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{att}\");");
                                writer.WriteLine("\t\t\t}");
                                writer.WriteLine("\t\t}");
                            }
                        }
                    }
                    #endregion

                    if (stateAtt != null)
                    {
                        #region generate std-statecode property
                        writer.WriteLine("\t\t[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute(\"statecode\")]");
                        writer.WriteLine($"\t\tpublic { logicalname }State? { stateAtt.SchemaName }");
                        writer.WriteLine("\t\t{");

                        writer.WriteLine("\t\t\tget");
                        writer.WriteLine("\t\t\t{");
                        writer.WriteLine("\t\t\t\tMicrosoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(\"statecode\");");
                        writer.WriteLine("\t\t\t\tif ((optionSet != null))");
                        writer.WriteLine("\t\t\t\t{");
                        writer.WriteLine($"\t\t\t\t\treturn (({ logicalname }State)(System.Enum.ToObject(typeof({ logicalname }State), optionSet.Value)));");
                        writer.WriteLine("\t\t\t\t}");
                        writer.WriteLine($"\t\t\t\telse");
                        writer.WriteLine("\t\t\t\t{");
                        writer.WriteLine("\t\t\t\t\treturn null;");
                        writer.WriteLine("\t\t\t\t}");
                        writer.WriteLine("\t\t\t}");

                        writer.WriteLine("\t\t\tset");
                        writer.WriteLine("\t\t\t{");
                        writer.WriteLine($"\t\t\t\tthis.OnPropertyChanging(\"{ stateAtt.SchemaName }\");");
                        writer.WriteLine("\t\t\t\tif ((value == null))");
                        writer.WriteLine("\t\t\t\t{");
                        writer.WriteLine("\t\t\t\t\tthis.SetAttributeValue(\"statecode\", null);");
                        writer.WriteLine("\t\t\t\t}");
                        writer.WriteLine("\t\t\t\telse");
                        writer.WriteLine("\t\t\t\t{");
                        writer.WriteLine("\t\t\t\t\tthis.SetAttributeValue(\"statecode\", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));");
                        writer.WriteLine("\t\t\t\t}");
                        writer.WriteLine($"\t\t\t\tthis.OnPropertyChanged(\"{ stateAtt.SchemaName }\");");
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
