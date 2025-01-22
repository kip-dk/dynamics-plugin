namespace Kipon.Xrm.ServiceAPI
{
    using Extensions.QueryExpression;
    using Extensions.Strings;
    using Extensions.TypeConverters;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;
    using System.Net;
    using Kipon.Xrm.Extensions.Json;

    public abstract class AbstractPostGRestService
    {
        private readonly IOrganizationService orgService;
        private ServiceAPI.IEntityMetadataService metaService;

        protected abstract string BaseURL { get; }

        protected abstract Guid ToId(string value);
        protected abstract string FromId(Guid id);

        protected virtual object Resolve(string externalname, object value)
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

        public Microsoft.Xrm.Sdk.EntityCollection Query(Microsoft.Xrm.Sdk.Query.QueryExpression query, params string[] quickfindfields)
        {
            var meta = this.metaService.ForEntity(query.EntityName);
            var url = this.GetEntityUrl(query.EntityName, meta.ExternalName);
            var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();

            var nextParam = "?";
            #region paging
            if (query.PageInfo != null && query.PageInfo.Count > 0)
            {
                url.Append($"{nextParam}limit={query.PageInfo.Count}&offset={(query.PageInfo.PageNumber - 1)}");
                nextParam = "&";
            }
            #endregion

            #region column selection
            if (query.ColumnSet.AllColumns != true)
            {
                var cols = (from a in meta.Attributes
                            join c in query.ColumnSet.Columns on a.LogicalName equals c
                            where a.ExternalName != null
                              && a.ExternalName != string.Empty
                            select a.ExternalName).Distinct().ToList();

                if (cols.Count > 0)
                {
                    if (!cols.Contains(key.ExternalName))
                    {
                        cols.Add(key.ExternalName);
                    }
                    url.Append($"{nextParam}select={string.Join(",", cols.ToArray())}");
                    nextParam = "&";
                }
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

                url.Append($"{nextParam}order={os.ToString()}");
                nextParam = "&";
            }
            #endregion

            #region quickfind filtering
            if (quickfindfields != null && quickfindfields.Length > 0)
            {
                var quickFilter = query.QuickFindFilter();
                if (!string.IsNullOrEmpty(quickFilter))
                {
                    var qf = new StringBuilder();
                    qf.Append("(");
                    var comma = "";
                    var filter = quickFilter.Replace("%", "*");
                    if (!filter.EndsWith("*"))
                    {
                        filter += "*";
                    }
                    foreach (var column in quickfindfields)
                    {
                        var attr = (from a in meta.Attributes
                                    where a.LogicalName == column
                                    select a).Single();
                        qf.Append($"{comma}{column}=like.{filter}");
                        comma = ",";
                    }
                    qf.Append(")");
                    url.Append($"{nextParam}or={qf.ToString()}");
                    nextParam = "&";
                }
            }
            #endregion

            #region column filtering
            if (query.Criteria != null && query.Criteria.Conditions != null && query.Criteria.Conditions.Count > 0)
            {
                foreach (var con in query.Criteria.Conditions)
                {
                    var next = con.ToPostGFilter(meta);
                    if (!string.IsNullOrEmpty(next))
                    {
                        url.Append($"{nextParam}{next}");
                        nextParam = "&";
                    }
                }
            }

            if (query.Criteria != null && query.Criteria.Filters != null && query.Criteria.Filters.Count > 0)
            {
                foreach (var filter in query.Criteria.Filters)
                {
                    var next = filter.ToPostGFilter(meta);
                    if (!string.IsNullOrEmpty(next))
                    {
                        url.Append($"{nextParam}{next}");
                        nextParam = "&";
                    }
                }
            }
            #endregion

            var results = this.Fetch(meta, url.ToString());
            var result = new EntityCollection();
            result.EntityName = query.EntityName;
            result.Entities.AddRange(results);
            result.MoreRecords = query.PageInfo != null && query.PageInfo.Count == results.Length;
            return result;
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
            }
            else
            {
                sb.Append(externalname);
            }
            return sb;
        }

        private Entity[] Fetch(Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta, string url)
        {
            Kipon.Xrm.Tracer.Trace($"PostG url: {url}"); 

            var req = WebRequest.Create(url);

            using (var resp = req.GetResponse())
            {
                using (var str = resp.GetResponseStream())
                {
                    var rows = str.ToDictionaryArray();
                    var result = new List<Entity>();

                    foreach (var row in rows)
                    {
                        var key = meta.Attributes.Where(r => r.IsPrimaryId == true).Single();
                        var val = row[key.ExternalName];
                        var id = this.ToId(val);
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
                                                    next[att.LogicalName] = decimal.Parse(value);
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Double:
                                                {
                                                    next[att.LogicalName] = double.Parse(value);
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Integer:
                                                {
                                                    next[att.LogicalName] = int.Parse(value);
                                                    break;
                                                }
                                            case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist:
                                                {
                                                    next[att.LogicalName] = new OptionSetValue(int.Parse(value));
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

    public static class AbstractPostGRestServiceLocalExtensions
    {
        public static string ToPostGFilter(this Microsoft.Xrm.Sdk.Query.FilterExpression fil, Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta)
        {
            var sb = new StringBuilder();
            if (fil.Conditions != null && fil.Conditions.Count > 0)
            {
                var nextParam = "";
                foreach (var con in fil.Conditions)
                {
                    var next = con.ToPostGFilter(meta);
                    if (!string.IsNullOrEmpty(next))
                    {
                        sb.Append($"{nextParam}{next}");
                    }

                }
            }
            return sb.ToString();
        }

        public static string ToPostGFilter(this Microsoft.Xrm.Sdk.Query.ConditionExpression con, Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta)
        {
            var value = con.Values.ToGFilterValue(con.Operator);
            if (value == null)
            {
                Kipon.Xrm.Tracer.Trace($"Unable to set filter: {con.AttributeName} {con.Operator}");
            }

            var att = meta.Attributes.Where(r => r.LogicalName == con.AttributeName).Single();
            var sb = new StringBuilder();
            sb.Append($"{att.LogicalName}=");

            switch (con.Operator)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Above:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterThan:
                    sb.Append($"gt.{con.Values.ToGFilterValue(con.Operator)}");
                    break;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.AboveOrEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual:
                    {
                        sb.Append($"gte.{con.Values.ToGFilterValue(con.Operator)}");
                        break;
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Under:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan:
                    {
                        sb.Append($"lt.{con.Values.ToGFilterValue(con.Operator)}");
                        break;
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.UnderOrEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual:
                    {
                        sb.Append($"lt.{con.Values.ToGFilterValue(con.Operator)}");
                        break;
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Like:
                    {
                        sb.Append($"like.{con.Values.ToGFilterValue(con.Operator)}");
                        break;
                    }

                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotBeginWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotEndWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotContain:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotLike:
                    {
                        sb.Append($"not.like.{con.Values.ToGFilterValue(con.Operator)}");
                        break;
                    }
                default:
                    {
                        Kipon.Xrm.Tracer.Trace($"condition on { con.AttributeName } { con.Operator } is not implemented");
                        break;
                    }
            }
            return sb.ToString();
        }

        public static string ToGFilterValue(this DataCollection<object> value, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.Null || opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual)
            {
                return string.Empty;
            }

            if (value == null || value.Count == 0)
            {
                return null;
            }

            if (value.Count > 1)
            {
                Kipon.Xrm.Tracer.Trace($"Multi values in condition is not implemented");
                return null;
            }

            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Above:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterThan:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.AboveOrEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal:
                    {
                        return value.First().ToString();
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Like:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotLike:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotBeginWith:
                    return value.First().ToString().Replace("%", "*").MustEndWith("*");
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotContain:
                    {
                        return value.First().ToString().Replace("%", "*").MustStartWith("*").MustEndWith("*");
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotEndWith:
                    {
                        return value.First().ToString().Replace("%", "*").MustStartWith("*");
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Null:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull:
                    {
                        return string.Empty;
                    }
                default:
                    Kipon.Xrm.Tracer.Trace($"Value operator: {opr} is not implemented");
                    return null;
            }
        }
    }
}
