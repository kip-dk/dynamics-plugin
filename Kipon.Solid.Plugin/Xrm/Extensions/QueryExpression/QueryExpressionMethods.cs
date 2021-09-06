namespace Kipon.Xrm.Extensions.QueryExpression
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class QueryExpressionMethods
    {
        [System.Diagnostics.DebuggerNonUserCode()]

        public static string QuickFindFilter(this Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            if (query.Criteria != null && query.Criteria.Filters != null && query.Criteria.Filters.Count > 0)
            {
                var qf = query.Criteria.Filters.Where(r => r.IsQuickFindFilter).SingleOrDefault();
                if (qf != null && qf.Conditions != null && qf.Conditions.Count > 0)
                {
                    var value = qf.Conditions[0].Values.Where(v => v is string).FirstOrDefault() as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        [System.Diagnostics.DebuggerNonUserCode()]

        public static T[] GetAttributeFilter<T>(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string attributeLogicalName)
        {
            var results = new List<T>();

            void Resolve(Microsoft.Xrm.Sdk.Query.FilterExpression filter)
            {
                if (filter.Conditions != null && filter.Conditions.Count > 0)
                {
                    var condition = filter.Conditions.Where(r => r.AttributeName == attributeLogicalName).FirstOrDefault();
                    if (condition != null)
                    {
                        foreach (var v in condition.Values)
                        {
                            var resolved = false;
                            var t = v.ConvertValueTo<T>(out resolved);
                            if (resolved && t != null)
                            {
                                results.Add(t);
                            }
                        }
                    }
                }
            }

            if (query != null && query.Criteria != null && query.Criteria.Conditions != null && query.Criteria.Conditions.Count > 0)
            {
                Resolve(query.Criteria);
                if (results.Count > 0)
                {
                    return results.ToArray();
                }
            }

            if (query.Criteria != null && query.Criteria.Filters != null && query.Criteria.Filters.Count > 0)
            {
                foreach (var filter in query.Criteria.Filters.Where(r => r.Conditions != null && r.Conditions.Count > 0))
                {
                    Resolve(filter);
                    if (results.Count > 0)
                    {
                        return results.ToArray();
                    }
                }
            }
            return null;
        }

        [System.Diagnostics.DebuggerNonUserCode()]

        public static Guid? GetIdFilter(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string logicalName)
        {
            if (!string.IsNullOrEmpty(logicalName) && query != null && query.Criteria != null && query.Criteria.Filters != null && query.Criteria.Filters.Count > 0)
            {
                var idFilter = query.Criteria.Conditions.Where(r => r.AttributeName == $"{logicalName}id").FirstOrDefault();
                if (idFilter != null && idFilter.Values != null && idFilter.Values.Count == 1)
                {
                    return idFilter.Values.First().ConvertValueTo<Guid>(out bool resolved);
                }
            }
            return null;
        }

        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ConvertValueTo<T>(this object o, out bool resolved)
        {
            resolved = true;
            if (o == null) return default(T);

            if (o is T t) return t;

            {
                if (o is Microsoft.Xrm.Sdk.EntityReference er && typeof(T) == typeof(Guid))
                {
                    return (T)(object)er.Id;
                }
            }

            {
                if (o is Microsoft.Xrm.Sdk.OptionSetValue er && typeof(T) == typeof(int))
                {
                    return (T)(object)er.Value;
                }
            }

            {
                if (o is Microsoft.Xrm.Sdk.Money er && typeof(T) == typeof(decimal))
                {
                    return (T)(object)er.Value;
                }
            }


            resolved = false;
            return default(T);
        }
    }
}
