namespace Kipon.Xrm.Models
{
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
            if (opr == ConditionOperator.Equal)
            {

            }
            return this.query;
        }
    }
}
