namespace Kipon.Xrm.Extensions.Entities
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EntitiesExtension
    {
        private static System.Globalization.CultureInfo CI = System.Globalization.CultureInfo.GetCultureInfo("en-US");

        public static Microsoft.Xrm.Sdk.Entity ToEntity(this Dictionary<string, string> row, Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta, Func<string, Guid> resolveId, Func<string, string, object> resolveValue)
        {
            var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();
            var val = row[key.ExternalName];
            var id = resolveId(val);
            var next = new Entity(meta.LogicalName);
            next[key.LogicalName] = id;

            foreach (var column in row.Keys)
            {
                var attrs = meta.Attributes.Where(r => r.IsPrimaryId != true && r.ExternalName == column).ToArray();
                if (attrs.Length > 0)
                {
                    var value = row[column];
                    if (value != null && !string.IsNullOrEmpty(value))
                    {
                        var raw = value;
                        foreach (var att in attrs)
                        {
                            var resolved = resolveValue(att.ExternalName, value);
                            if (resolved != null)
                            {
                                next[att.LogicalName] = resolved;
                                continue;
                            }

                            switch (att.AttributeType)
                            {
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Boolean:
                                    {
                                        var lower = raw.ToLower();
                                        next[att.LogicalName] = lower == "true" || lower == "on" || lower == "yes" || lower == "ja" || lower == "ok" || lower == "1";
                                        continue;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.BigInt:
                                    {
                                        next[att.LogicalName] = long.Parse(raw);
                                        continue;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.DateTime:
                                    {
                                        next[att.LogicalName] = DateTime.Parse(value);
                                        continue;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.String:
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Memo:
                                    {
                                        next[att.LogicalName] = value;
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Decimal:
                                    {
                                        next[att.LogicalName] = decimal.Parse(value.Replace(",", "."), CI);
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Double:
                                    {
                                        next[att.LogicalName] = double.Parse(value.Replace(",", "."), CI);
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Integer:
                                    {
                                        next[att.LogicalName] = int.Parse(value);
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist:
                                    {
                                        if (att is MultiSelectPicklistAttributeMetadata ms)
                                        {
                                            var col = new OptionSetValueCollection(value.Split(',').Select(r => new OptionSetValue(int.Parse(r.Trim()))).ToArray());
                                            next[att.LogicalName] = col;
                                        }
                                        else
                                        {
                                            next[att.LogicalName] = new OptionSetValue(int.Parse(value));
                                        }
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Money:
                                    {
                                        next[att.LogicalName] = new Money(decimal.Parse(value));
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.State:
                                    {
                                        next[att.LogicalName] = new OptionSetValue(int.Parse(value));
                                        break;
                                    }
                                case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Lookup:
                                    {
                                        var spl = value.Split(':');
                                        if (spl.Length >= 2)
                                        {
                                            var lookup = new Microsoft.Xrm.Sdk.EntityReference(spl[0], new Guid(spl[1]));
                                            if (spl.Length > 2)
                                            {
                                                var last = value.Substring(spl[0].Length + spl[1].Length + 2).Trim();
                                                if (!string.IsNullOrEmpty(last))
                                                {
                                                    lookup.Name = last;
                                                }
                                            }
                                            next[att.LogicalName] = lookup;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return next;
        }

        public static Microsoft.Xrm.Sdk.Entity[] ToEntities(this Dictionary<string, string>[] inputs, Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta, Func<string, Guid> resolveId, Func<string, string, object> resolveValue)
        {
            var result = new List<Microsoft.Xrm.Sdk.Entity>();

            foreach (var input in inputs)
            {
                result.Add(input.ToEntity(meta, resolveId, resolveValue));
            }
            return result.ToArray();
        }

        public static Dictionary<string,string> ToDictionary(this Microsoft.Xrm.Sdk.Entity entity)
        {
            var result = new Dictionary<string, string>();

            foreach (var attr in entity.Attributes.Keys)
            {
                var v = XrmValueToString(entity[attr]);
                if (v != null)
                {
                    result[attr] = v;
                }
            }
            return result;
        }

        public static Dictionary<string, string>[] ToDictionaryList(this Microsoft.Xrm.Sdk.EntityCollection entityCollection)
        {
            var result = new List<Dictionary<string, string>>();
            foreach (var entity in entityCollection.Entities)
            {
                result.Add(entity.ToDictionary());
            }
            return result.ToArray();
        }

        private static string XrmValueToString(this object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string s)
            {
                return s;
            }

            if (value is int i)
            {
                return i.ToString();
            }

            if (value is long l)
            {
                return l.ToString();
            }

            if (value is double db)
            {
                return db.ToString(CI);
            }

            if (value is decimal de)
            {
                return de.ToString(CI);
            }

            if (value is bool bo)
            {
                return bo == true ? "true" : "false";
            }

            if (value is DateTime dt)
            {
                return dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            if (value is OptionSetValue os)
            {
                return os.Value.ToString();
            }

            if (value is OptionSetValueCollection oc)
            {
                if (oc.Count > 0)
                {
                    return string.Join(",", oc.Select(r => r.Value.ToString()));
                }
                return null;
            }

            if (value is Money me)
            {
                return me.Value.ToString(CI);
            }

            if (value is EntityReference re)
            {
                return $"{ re.LogicalName }:{ re.Id.ToString() }: { re.Name }";
            }
            return null;
        }
    }
}
