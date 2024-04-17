namespace Kipon.Xrm.Extensions.QueryExpression
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using TypeConverters;

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
        public static Guid? EntityReferenceIdEqualFilter(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string attribName)
        {
            var vs = query.FilterValues<Guid>(attribName, Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal);
            if (vs != null && vs.Length > 0)
            {
                return vs.First();
            }
            return null;
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static Guid[] EntityReferenceIdInFilter(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string attribName)
        {
            return query.FilterValues<Guid>(attribName, Microsoft.Xrm.Sdk.Query.ConditionOperator.In);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static int[] OptionSetValueInFilter(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string attribName)
        {
            return query.FilterValues<int>(attribName, Microsoft.Xrm.Sdk.Query.ConditionOperator.In);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static T[] FilterValues<T>(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string attribName, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            return query.Criteria.FilterValues<T>(attribName, opr);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static T[] FilterValues<T>(this Microsoft.Xrm.Sdk.Query.FilterExpression filter, string attribName, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            if (filter != null && filter.Conditions != null && filter.Conditions.Count > 0)
            {
                var con = filter.Conditions.Where(r => r.AttributeName == attribName && r.Operator == Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal && r.Values != null && r.Values.Count > 1).FirstOrDefault();

                if (con != null)
                {
                    return con.Values.Select(r => (T)r).ToArray();
                }

                if (filter.Filters != null)
                {
                    var next = filter.FilterValues<T>(attribName, opr);
                    if (next != null)
                    {
                        return next;
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
        public static Microsoft.Xrm.Sdk.Query.QueryExpression Trace(this Microsoft.Xrm.Sdk.Query.QueryExpression query, Microsoft.Xrm.Sdk.ITracingService tracingService)
        {
            if (query.Criteria == null)
            {
                tracingService.Trace($"Query for: { query.EntityName } did not have any conditions");
                return query;
            }

            if (query.Criteria != null)
            {
                query.Criteria.Trace(tracingService, 1);
            }

            return query;
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static Microsoft.Xrm.Sdk.Query.FilterExpression Trace(this Microsoft.Xrm.Sdk.Query.FilterExpression filter, Microsoft.Xrm.Sdk.ITracingService tracingService, int indent)
        {
            var indentString = "".PadLeft(indent, ' ');
            if (filter.IsQuickFindFilter)
            {
                tracingService.Trace($"{ indentString }Filter i quickfindfilter");
            }

            if (filter.Conditions != null && filter.Conditions.Count > 0)
            {
                foreach (var con in filter.Conditions)
                {
                    tracingService.Trace($"{ indentString } { con.EntityName }.{con.AttributeName} { con.Operator }");
                    if (con.Values != null && con.Values.Count > 0)
                    {
                        foreach (var val in con.Values)
                        {
                            tracingService.Trace($"{ indentString }  { val.GetType().FullName } { val.ToString() }");
                        }
                    }
                }
            }

            if (filter.Filters != null && filter.Filters.Count > 0)
            {
                foreach (var sub in filter.Filters)
                {
                    sub.Trace(tracingService, indent + 1);
                }
            }
            return filter;
        }
    }
}
