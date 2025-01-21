namespace Kipon.Xrm.ServiceAPI
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public abstract class AbstractPostGRestService
    {
        private readonly IOrganizationService orgService;
        private ServiceAPI.IEntityMetadataService metaService;

        protected abstract string BaseURL { get; }

        protected abstract Guid ToId(string value);
        protected abstract string FromId(Guid id);

        protected virtual object Resolve(string externalname, JsonElement value)
        {
            return null;
        }

        public AbstractPostGRestService(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
            this.metaService = new Services.EntityMetadataService(orgService);

        }

        public Microsoft.Xrm.Sdk.Entity Get(string logicalName, Guid id)
        {
            var meta = this.metaService.ForEntity(logicalName);
            var url = this.GetEntityUrl(logicalName, meta.ExternalName);

            var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();
            var val = this.FromId(id);
            url.Append($"?{key.ExternalName}=eq.{val.ToString()}");

            var entities = this.Fetch(meta, url.ToString());
            return entities.First();
        }

        public Microsoft.Xrm.Sdk.EntityCollection Query(Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            var meta = this.metaService.ForEntity(query.EntityName);
            var url = this.GetEntityUrl(query.EntityName, meta.ExternalName);
            var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();

            var nextParam = "?";
            #region paging
            if (query.PageInfo != null && query.PageInfo.Count > 0)
            {
                url.Append($"{ nextParam }limit={ query.PageInfo.Count }&offset={ (query.PageInfo.PageNumber - 1) }");
                nextParam = "&";
            }
            #endregion

            #region column selection
            if (query.ColumnSet.AllColumns != true)
            {
                var cols = (from a in meta.Attributes
                            join c in query.ColumnSet.Columns on a.ExternalName equals c
                            select a.ExternalName).Distinct().ToList();
                if (!cols.Contains(key.ExternalName))
                {
                    cols.Add(key.ExternalName);
                }
                url.Append($"{nextParam}{string.Join(",", cols.ToArray())}");
                nextParam = "&";
            }
            #endregion

            #region orderby
            if (query.Orders != null && query.Orders.Count > 0)
            {
                var os = new StringBuilder();
                var comma = "";
                foreach (var order in query.Orders)
                {
                    var orderAttr = meta.Attributes.Where(r => r.IsPrimaryId != true && r.LogicalName == order.AttributeName).SingleOrDefault();
                    if (orderAttr != null)
                    {
                        os.Append($"{comma}{orderAttr.ExternalName}");
                        if (order.OrderType == Microsoft.Xrm.Sdk.Query.OrderType.Descending)
                        {
                            os.Append(".desc");
                        }
                    }
                    comma = ",";
                }
            }
            #endregion

            #region filtering
            // resolve the filter
            #endregion

            var results = this.Fetch(meta, url.ToString());
            var result = new EntityCollection();
            result.EntityName = query.EntityName;
            result.Entities.AddRange(results);
            return null;
        }


        private StringBuilder GetEntityUrl(string logicalname, string externalname)
        {
            var sb = new StringBuilder();
            sb.Append(this.BaseURL);
            if (!this.BaseURL.EndsWith("/"))
            {
                sb.Append("/");
            }

            if (string.IsNullOrEmpty(externalname))
            {
                sb.Append(logicalname);
            } else
            {
                sb.Append(externalname);
            }
            return sb;
        }

        private Entity[] Fetch(Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta, string url)
        {
            var req = WebRequest.Create(url);

            using (var resp = req.GetResponse()) 
            {
                using (var str = resp.GetResponseStream())
                {
                    var rows = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, JsonElement>[]>(str);
                    var result = new List<Entity>();

                    foreach (var row in rows)
                    {
                        var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();
                        var val = row[key.ExternalName];
                        var id = this.ToId(val.GetRawText());
                        var next = new Entity(meta.LogicalName);
                        next[key.LogicalName] = id;

                        foreach (var column in row.Keys)
                        {
                            var attrs = meta.Attributes.Where(r => r.IsPrimaryId != true && r.ExternalName == column).ToArray();
                            if (attrs.Length > 0)
                            {
                                var value = row[column];
                                if (value.ValueKind != JsonValueKind.Null && !string.IsNullOrEmpty(value.GetRawText()))
                                {
                                    var raw = value.GetRawText();
                                    foreach (var att in attrs)
                                    {
                                        var resolved = this.Resolve(att.ExternalName, value);
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
                                                    next[att.LogicalName] = value.GetDateTime();
                                                    continue;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.String:
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Memo:
                                                {
                                                    next[att.LogicalName] = value.GetString();
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Decimal:
                                                {
                                                    next[att.LogicalName] = value.GetDecimal();
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Double:
                                                {
                                                    next[att.LogicalName] = value.GetDouble();
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Integer:
                                                {
                                                    next[att.LogicalName] = value.GetInt32();
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist:
                                                {
                                                    next[att.LogicalName] = new OptionSetValue(value.GetInt32());
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Money:
                                                {
                                                    next[att.LogicalName] = new Money(value.GetDecimal());
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.State:
                                                {
                                                    next[att.LogicalName] = new OptionSetValue(value.GetInt32());
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
                        result.Add(next);
                    }
                    return result.ToArray();
                }
            }
        }
    }
}
