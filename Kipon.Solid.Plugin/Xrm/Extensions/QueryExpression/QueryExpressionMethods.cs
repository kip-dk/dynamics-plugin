﻿namespace Kipon.Xrm.Extensions.QueryExpression
{
    using Extensions.DateTimes;
    using Implementations;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.CodeDom;
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
        public static Microsoft.Xrm.Sdk.EntityCollection Query<T>(this ICollection<T> collection, Microsoft.Xrm.Sdk.Query.QueryExpression expression, params string[] quickFindFields) where T: Microsoft.Xrm.Sdk.Entity
        {
            var handler = new Implementations.QueryResolver<T>(collection, expression, quickFindFields);
            return handler.Resolve();
        }

        // [System.Diagnostics.DebuggerNonUserCode()]
        public static Microsoft.Xrm.Sdk.Entity[] Sort(this Microsoft.Xrm.Sdk.Entity[] values, Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            if (query != null && query.Orders != null && query.Orders.Count > 0)
            {
                return values.Select(r => new Sorter(r, query.Orders)).OrderBy(s => s).Select(s => s.entity).ToArray();
            }
            return values.ToArray();
        }

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
        public static Microsoft.Xrm.Sdk.Query.ConditionOperator ToConditionOperator(this string value, out string rawsearch)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidPluginExecutionException($"Null or empty string cannot be resolved to a ConditionOperator");
            }

            if (value.StartsWith("%") && value.EndsWith("%"))
            {
                rawsearch = value.Substring(1, value.Length - 2);
                return Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains;
            }

            if (value.StartsWith("%") && !value.EndsWith("%"))
            {
                rawsearch = value.Substring(1, value.Length - 1);
                return Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith;
            }

            if (!value.StartsWith("%") && value.EndsWith("%"))
            {
                rawsearch = value.Substring(0, value.Length - 1);
                return Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith;
            }

            rawsearch = value;
            return Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal;
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
                var con = filter.Conditions.Where(r => r.AttributeName == attribName && r.Operator == opr && r.Values != null && r.Values.Count > 0).FirstOrDefault();

                if (con != null)
                {
                    return con.Values.Select(r => (T)r).ToArray();
                }

                if (filter.Filters != null)
                {
                    foreach (var sub in filter.Filters)
                    {
                        var next = sub.FilterValues<T>(attribName, opr);
                        if (next != null)
                        {
                            return next;
                        }
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
                tracingService.Trace($"{ indentString } [{ indent }] Filter is quickfindfilter:");
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

        [System.Diagnostics.DebuggerNonUserCode()]
        public static object SafeValueOf(this Microsoft.Xrm.Sdk.Entity entity, string attrName)
        {
            if (entity.Attributes.ContainsKey(attrName))
            {
                var val = entity[attrName];

                if (val is Microsoft.Xrm.Sdk.EntityReference re)
                {
                    return re.Name;
                }

                return val;
            }
            return null;
        }

        public static bool Match(this Microsoft.Xrm.Sdk.Entity entity, Microsoft.Xrm.Sdk.Query.FilterExpression expression)
        {
            if ((expression.Conditions == null || expression.Conditions.Count == 0) && (expression.Filters == null || expression.Filters.Count == 0))
            {
                return true;
            }

            switch (expression.FilterOperator)
            {
                case Microsoft.Xrm.Sdk.Query.LogicalOperator.And: return entity.MatchAnd(expression);
                case Microsoft.Xrm.Sdk.Query.LogicalOperator.Or: return entity.MatchOr(expression);
                default: throw new InvalidPluginExecutionException($"Unsupported operator: [{ expression.FilterOperator }]");
            }
        }

        public static bool MatchAnd(this Microsoft.Xrm.Sdk.Entity entity, Microsoft.Xrm.Sdk.Query.FilterExpression expression)
        {
            var resolveFilters = expression.Filters?.Where(r => !r.IsQuickFindFilter).ToArray();
            if ((resolveFilters == null || resolveFilters.Length == 0) && (expression.Conditions == null || expression.Conditions.Count == 0)) 
            {
                return true;
            }

            if (expression.Conditions != null && expression.Conditions.Count > 0)
            {
                foreach (var condition in expression.Conditions)
                {
                    object value = null;
                    if (entity.Attributes.Contains(condition.AttributeName))
                    {
                        value = entity[condition.AttributeName];
                    }
                    var next = condition.IsTrue(value);
                    if (!next)
                    {
                        return false;
                    }
                }
            }

            if (resolveFilters != null && resolveFilters.Length > 0)
            {
                foreach (var filter in resolveFilters)
                {
                    var next = entity.Match(filter);
                    if (!next)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool MatchOr(this Microsoft.Xrm.Sdk.Entity entity, Microsoft.Xrm.Sdk.Query.FilterExpression expression)
        {
            var resolveFilters = expression.Filters?.Where(r => !r.IsQuickFindFilter).ToArray();
            if ((resolveFilters == null || resolveFilters.Length == 0) && (expression.Conditions == null || expression.Conditions.Count == 0))
            {
                return true;
            }

            if (expression.Conditions != null && expression.Conditions.Count > 0)
            {
                foreach (var condition in expression.Conditions)
                {
                    object value = null;
                    if (entity.Attributes.Contains(condition.AttributeName))
                    {
                        value = entity[condition.AttributeName];
                    }
                    var next = condition.IsTrue(value);
                    if (next)
                    {
                        return true;
                    }
                }
            }

            if (resolveFilters != null && resolveFilters.Length > 0)
            {
                foreach (var filter in resolveFilters)
                {
                    var next = entity.Match(filter);
                    if (next)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsTrue(this Microsoft.Xrm.Sdk.Query.ConditionExpression condition, object enttyObjectValue)
        {
            return condition.Operator.IsTrue(condition.Values?.ToArray(), enttyObjectValue);
        }

        public static bool IsTrue(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, object[] values, object entityObjectValue)
        {
            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.Null && entityObjectValue == null)
            {
                return true;
            }

            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull && entityObjectValue != null)
            {
                return true;
            }

            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.In)
            {
                return values.IsIn(entityObjectValue);
            }

            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.NotIn)
            {
                return values.IsNotIn(entityObjectValue);
            }

            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.Between)
            {
                return entityObjectValue.IsBetween(values);
            }

            if (opr == Microsoft.Xrm.Sdk.Query.ConditionOperator.NotBetween)
            {
                return !entityObjectValue.IsBetween(values);
            }



            if (entityObjectValue is DateTime dt && (values == null || values.Length != 1 || !(values[0] is DateTime)))
            {
                return opr.CompareSpecialDate(values, dt);
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.OptionSetValueCollection evc && values != null && values.Length > 0)
            {
                return opr.CompareOptionSetValueCollection(values.Select(r => (int)r).OrderBy(r => r).ToArray(), evc);
            }

            if (entityObjectValue == null && values != null && values.Length > 0)
            {
                return false;
            }

            if (values != null && values.Length > 0)
            {
                foreach (var filterValue in values)
                {
                    var next = opr.IsTrue(filterValue, entityObjectValue);
                    if (next)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CompareOptionSetValueCollection(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, int[] filterValues, Microsoft.Xrm.Sdk.OptionSetValueCollection entityObjectValue)
        {
            var entityObjectValues = entityObjectValue.Select(r => r.Value).OrderBy(r =>r).ToArray();

            switch (opr)
            {
                case ConditionOperator.Equal:
                    {
                        if (entityObjectValues.Length != filterValues.Length) return false;
                        for (var i=0;i<filterValues.Length;i++)
                        {
                            if (entityObjectValues[i] != filterValues[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case ConditionOperator.NotEqual:
                    {
                        if (entityObjectValues.Length != filterValues.Length) return true;
                        for (var i = 0; i < filterValues.Length; i++)
                        {
                            if (entityObjectValues[i] != filterValues[i])
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case ConditionOperator.ContainValues:
                    {
                        foreach (var v in filterValues)
                        {
                            if (!entityObjectValues.Contains(v))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case ConditionOperator.DoesNotContainValues:
                    {
                        foreach (var v in filterValues)
                        {
                            if (entityObjectValues.Contains(v))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                default: throw new InvalidPluginExecutionException($"Unexpected OptionSetValueCollection operator: {opr}");
            }
        }

        public static bool IsTrue(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, object filterValue, object entityObjectValue)
        {
            if (entityObjectValue is int va)
            {
                return opr.CompareInt((int)filterValue, va);
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.OptionSetValue os)
            {
                return opr.CompareInt((int)filterValue, os.Value);
            }

            if (entityObjectValue is decimal de)
            {
                return opr.CompareDecimal((decimal)filterValue, de);
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.Money mo)
            {
                return opr.CompareDecimal((int)filterValue, mo.Value);
            }

            if (entityObjectValue is DateTime ev && filterValue is DateTime fv)
            {
                return opr.CompareDate(fv, ev);
            }

            if (entityObjectValue is string sev && filterValue is string fev)
            {
                return opr.CompareString(fev, sev);
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.EntityReference entityref && filterValue is Guid gv)
            {
                return opr.CompareGuid(gv, entityref.Id);
            }

            if (entityObjectValue is Guid entityId && filterValue is Guid gfv)
            {
                return opr.CompareGuid(gfv, entityId);
            }

            if (entityObjectValue is bool trueFalse && filterValue != null)
            {
                return opr.CompareBool(filterValue.ToBool(), trueFalse);
            }

            throw new InvalidPluginExecutionException($"Expression not supported: [{ opr }] [{ entityObjectValue }] / [{filterValue}]");
        }

        public static bool IsBetween(this object entityObjectValue, object[] filterValue)
        {
            if (filterValue.Length != 2)
            {
                throw new InvalidPluginExecutionException($"Expected 2 filter values in between compare");
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.Money mo)
            {
                return mo.Value.IsBetween(filterValue);
            }

            if (entityObjectValue is Microsoft.Xrm.Sdk.OptionSetValue os)
            {
                return os.Value.IsBetween(filterValue);
            }

            if (entityObjectValue is decimal fv)
            {
                return fv >= (decimal)filterValue[0] && fv <= (decimal)filterValue[1];
            }

            if (entityObjectValue is int iv)
            {
                return iv >= (int)filterValue[0] && iv <= (int)filterValue[1];
            }

            if (entityObjectValue is float flv)
            {
                return flv >= (float)filterValue[0] && flv <= (float)filterValue[1];
            }

            if (entityObjectValue is double dbl)
            {
                return dbl >= (float)filterValue[0] && dbl <= (float)filterValue[1];
            }

            if (entityObjectValue is long lon)
            {
                return lon >= (long)filterValue[0] && lon <= (long)filterValue[1];
            }

            if (entityObjectValue is DateTime dt)
            {
                return dt >= (DateTime)filterValue[0] && dt <= (DateTime)filterValue[1];
            }

            throw new InvalidPluginExecutionException($"Unexpected datatype for bewteen condition: {entityObjectValue.GetType().FullName}");
        }

        public static bool IsIn(this object[] filterValues, object entityValue)
        {
            if (entityValue == null)
            {
                return false;
            }

            if (entityValue is Microsoft.Xrm.Sdk.OptionSetValue ov)
            {
                return filterValues.IsIn(ov.Value);
            }

            if (entityValue is Microsoft.Xrm.Sdk.Money mo)
            {
                return filterValues.IsIn(mo.Value);
            }

            if (entityValue is Microsoft.Xrm.Sdk.EntityReference re)
            {
                return filterValues.IsIn(re.Id);
            }

            foreach (var fv in filterValues)
            {
                var same = fv.IsSame(entityValue);
                if (same)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotIn(this object[] filterValues, object entityValue)
        {
            if (entityValue == null)
            {
                return false;
            }

            return !filterValues.IsIn(entityValue);
        }

        public static bool CompareGuid(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, Guid filterValue, Guid entityObjectValue)
        {
            switch (opr)
            {
                case ConditionOperator.Equal: return entityObjectValue == filterValue;
                case ConditionOperator.NotEqual: return entityObjectValue != filterValue;
                default: throw new InvalidPluginExecutionException($"Unexpected Guid operator: {opr}");
            }
        }

        public static bool CompareInt(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, int filterValue, int entityObjectValue)
        {
            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal: return entityObjectValue == filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual: return entityObjectValue >= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterThan: return entityObjectValue > filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual: return entityObjectValue <= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan: return entityObjectValue < filterValue;
                default:  throw new InvalidPluginExecutionException($"Unexpected int operator: { opr }");
            }
        }

        public static bool CompareDecimal(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, decimal filterValue, decimal entityObjectValue)
        {
            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal: return entityObjectValue == filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual: return entityObjectValue >= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterThan: return entityObjectValue > filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual: return entityObjectValue <= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan: return entityObjectValue < filterValue;
                default: throw new InvalidPluginExecutionException($"Unexpected decimal operator: {opr}");
            }
        }

        public static bool CompareString(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, string filterValue, string entityObjectValue)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrEmpty(entityObjectValue))
            {
                throw new InvalidPluginExecutionException($"Expected to have a value for filter and object, filter: [{ filterValue }], object: [{ entityObjectValue }]");
            }

            filterValue = filterValue.ToLower();
            entityObjectValue = entityObjectValue.ToLower();

            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith: return entityObjectValue.StartsWith(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains: return entityObjectValue.Contains(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotBeginWith: return !entityObjectValue.StartsWith(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotContain: return !entityObjectValue.Contains(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotEndWith: return !entityObjectValue.EndsWith(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal: return entityObjectValue == filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Like: return filterValue.Like(entityObjectValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotLike: return !filterValue.Like(entityObjectValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith: return entityObjectValue.EndsWith(filterValue);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual: return entityObjectValue != filterValue;
                default: throw new InvalidPluginExecutionException($"Unexpected string operator: { opr }");
            }
        }

        public static bool CompareDate(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, DateTime filterValue, DateTime entityObjectValue)
        {
            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal: return entityObjectValue == filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual: return entityObjectValue >= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.OnOrAfter: return entityObjectValue >= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual: return entityObjectValue <= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.OnOrBefore: return entityObjectValue <= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Above: return entityObjectValue > filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.AboveOrEqual: return entityObjectValue >= filterValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.On: return entityObjectValue.StartOfDay() == filterValue.StartOfDay();
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotOn: return entityObjectValue.StartOfDay() != filterValue.StartOfDay();
                default: throw new InvalidPluginExecutionException($"Unexpected DateTime operator: {opr}");
            }
        }

        public static bool CompareSpecialDate(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, object[] filterValue, DateTime entityObjectValue)
        {
            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Today: return System.DateTime.UtcNow.StartOfDay() == entityObjectValue.StartOfDay();
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Last7Days: return System.DateTime.UtcNow.StartOfDay().AddDays(-7) <= entityObjectValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LastXDays: return System.DateTime.UtcNow.StartOfDay().AddDays((-1) * (int)filterValue[0]) <= entityObjectValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LastMonth: return (System.DateTime.UtcNow.Month - 1) == entityObjectValue.Month;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.LastXWeeks: return System.DateTime.UtcNow.StartOfDay().AddDays((-1) * 7 * (int)filterValue[0]) <= entityObjectValue;
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.ThisWeek:
                    {
                        var week = System.DateTime.UtcNow.Week();
                        return entityObjectValue >= week[0] && entityObjectValue <= week[1];
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.InFiscalYear:
                    {
                        var from = Models.Calendar.Current.YearStart;
                        var to = from.AddYears(1).AddMilliseconds(-1);
                        return entityObjectValue >= from && entityObjectValue <= to;
                    }
                default: throw new InvalidPluginExecutionException($"Special DateTime operator not implemented: {opr}");
            }
        }

        public static bool CompareBool(this Microsoft.Xrm.Sdk.Query.ConditionOperator opr, bool filterValue, bool entityObjectValue)
        {
            switch (opr)
            {
                case ConditionOperator.Equal:
                    {
                        return filterValue == entityObjectValue;
                    }
                case ConditionOperator.NotEqual:
                    {
                        return filterValue != entityObjectValue;
                    }
                default:
                    {
                        throw new InvalidPluginExecutionException($"Unexpected operator for True/False comparator: {opr}");
                    }
            }
        }

        public static bool Like(this string quickFindFilter, string entityObjectValue)
        {
            if (string.IsNullOrEmpty(quickFindFilter))
            {
                return true;
            }

            if (string.IsNullOrEmpty(entityObjectValue))
            {
                return false;
            }

            quickFindFilter = quickFindFilter.Replace("*", "%");

            if (!quickFindFilter.Contains("%"))
            {
                quickFindFilter = $"%{quickFindFilter}%";
            }

            var opr = Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith;

            if (quickFindFilter.StartsWith("%") && quickFindFilter.EndsWith("%"))
            {
                opr = ConditionOperator.Contains;
            }

            if (quickFindFilter.StartsWith("%") && !quickFindFilter.EndsWith("%"))
            {
                opr = ConditionOperator.EndsWith;
            }

            var finalFilter = quickFindFilter.Replace("%", "");

            return opr.CompareString(finalFilter, entityObjectValue);
        }

        public static bool QuickFindMatch(this Microsoft.Xrm.Sdk.Entity entity, string quickFindFilter, string[] quickFindFields)
        {
            if (!string.IsNullOrEmpty(quickFindFilter) && (quickFindFields != null && quickFindFields.Length > 0))
            {
                foreach (var field in quickFindFields)
                {
                    if (entity.Attributes.ContainsKey(field))
                    {
                        var value = entity[field];

                        if (value == null)
                        {
                            continue;
                        }

                        if (value is string stringValue)
                        {
                            if (string.IsNullOrEmpty(stringValue))
                            {
                                continue;
                            }

                            if (quickFindFilter.Like(stringValue))
                            {
                                return true;
                            }
                            continue;
                        }

                        if (value is Microsoft.Xrm.Sdk.EntityReference re)
                        {
                            if (re == null)
                            {
                                continue;
                            }

                            if (!string.IsNullOrEmpty(re.Name))
                            {
                                if (quickFindFilter.Like(re.Name))
                                {
                                    return true;
                                }
                            }
                            continue;
                        }

                        throw new InvalidPluginExecutionException($"Unsupported type in quickfind filter: { value?.GetType() }");
                    }
                }
                return false;
            }
            return true;
        }

        public static Microsoft.Xrm.Sdk.Query.QueryExpression ParseAndReplace(this Microsoft.Xrm.Sdk.Query.QueryExpression query, string field, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            var ev = new Models.QueryExpressionEvaluator(query);
            return ev.ParseAndReplace(field, opr);
        }
    }
}
