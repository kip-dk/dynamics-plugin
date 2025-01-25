namespace Kipon.Xrm.Models
{
    using Extensions.TypeConverters;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class QueryExpressionEvaluator
    {
        private readonly QueryExpression query;

        public QueryExpressionEvaluator(Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            this.query = query;
        }

        public Microsoft.Xrm.Sdk.Query.QueryExpression ParseAndReplace(string specialQueryField, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            switch (opr)
            {
                case ConditionOperator.Equal:
                    {
                        this.Replace(specialQueryField, this.query.Criteria, opr);
                        break;
                    }
                default:
                    {
                        throw new InvalidPluginExecutionException($"Operator: { opr } is not supported in QueryExpressionEvaluator");
                    }
            }
            return this.query;
        }

        private void Replace(string queryField, Microsoft.Xrm.Sdk.Query.FilterExpression filter, Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            if (filter != null && filter.Conditions.Count > 0)
            {
                foreach (var con in filter.Conditions.Where(r => r.AttributeName == queryField && r.Operator == opr).ToArray())
                {
                    if (con.Values != null && con.Values.Count == 1 && con.Values[0] is string conditionString && !string.IsNullOrEmpty(conditionString))
                    {
                        filter.Conditions.Remove(con);
                        if (conditionString.StartsWith("?"))
                        {
                            conditionString = conditionString.Substring(1);
                        }

                        var conditions = conditionString.Split('&');
                        foreach (var condition in conditions)
                        {
                            var field = condition.Split('=').First();
                            var val = this.ParseCondition(condition.Substring(field.Length + 1), out ConditionOperator resultOperator);

                            var next = new ConditionExpression
                            {
                                AttributeName = field,
                                Operator = resultOperator
                            };
                            next.Values.Add(val);

                            filter.Conditions.Add(next);
                        }
                    } 
                }
            }

            if (filter.Filters != null && filter.Filters.Count > 0)
            {
                foreach (var sub in filter.Filters)
                {
                    this.Replace(queryField, sub, opr);
                }
            }
        }

        private object ParseCondition(string v, out Microsoft.Xrm.Sdk.Query.ConditionOperator opr)
        {
            var not = false;
            object result = null;
            opr = ConditionOperator.Equal;

            var expressions = v.Split('.').Select(r => r.ToLower());

            foreach (var expression in expressions)
            {
                #region not
                if (expression == "not")
                {
                    not = true;
                    continue;
                }
                #endregion

                #region operator
                if (expression == "eq")
                {
                    opr = not ? ConditionOperator.NotEqual : ConditionOperator.Equal;
                    continue;
                }

                if (expression == "lt")
                {
                    if (not)
                    {
                        throw new InvalidPluginExecutionException($"no support for [not] on { expression }");
                    }
                    opr = ConditionOperator.LessThan;
                    continue;
                }

                if (expression == "lte")
                {
                    if (not)
                    {
                        throw new InvalidPluginExecutionException($"no support for [not] on {expression}");
                    }
                    opr = ConditionOperator.LessEqual;
                    continue;
                }

                if (expression == "gt")
                {
                    if (not)
                    {
                        throw new InvalidPluginExecutionException($"no support for [not] on {expression}");
                    }
                    opr = ConditionOperator.GreaterThan;
                    continue;
                }

                if (expression == "gte")
                {
                    if (not)
                    {
                        throw new InvalidPluginExecutionException($"no support for [not] on {expression}");
                    }
                    opr = ConditionOperator.GreaterEqual;
                    continue;
                }

                if (expression == "like")
                {
                    opr = not ? ConditionOperator.NotLike : ConditionOperator.Like;
                    continue;
                }
                #endregion

                #region date value handlers
                if (expression == "utc")
                {
                    result = System.DateTime.UtcNow;
                    continue;
                }

                if (expression == "now")
                {
                    result = System.DateTime.Now;
                    continue;
                }

                if (expression == "startofyear")
                {
                    if (result == null)
                    {
                        result = System.DateTime.UtcNow;
                    }

                    var baseValue = (DateTime)result;
                    result = new System.DateTime(baseValue.Year, 1, 1, 0, 0, 0, baseValue.Kind);
                    continue;
                }

                if (expression.StartsWith("addyears("))
                {
                    result = ((DateTime)result).AddYears(expression.ExtractConditionValue());
                    continue;
                }

                if (expression.StartsWith("addmonths("))
                {
                    result = ((DateTime)result).AddMonths(expression.ExtractConditionValue());
                    continue;
                }

                if (expression.StartsWith("adddays("))
                {
                    result = ((DateTime)result).AddMonths(expression.ExtractConditionValue());
                    continue;
                }

                if (expression.StartsWith("addhours("))
                {
                    result = ((DateTime)result).AddHours(expression.ExtractConditionValue());
                    continue;
                }

                if (expression.StartsWith("addminutes("))
                {
                    result = ((DateTime)result).AddMinutes(expression.ExtractConditionValue());
                    continue;
                }

                if (expression.StartsWith("addseconds("))
                {
                    result = ((DateTime)result).AddSeconds(expression.ExtractConditionValue());
                    continue;
                }
                #endregion
            }
            return result;
        }
    }
}
