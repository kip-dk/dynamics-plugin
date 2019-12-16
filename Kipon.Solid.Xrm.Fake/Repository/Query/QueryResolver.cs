using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResolver
    {
        internal Microsoft.Xrm.Sdk.EntityCollection ExecuteQuery(Microsoft.Xrm.Sdk.Query.QueryBase query, EntityShadow[] allEntities)
        {
            var qe = query as Microsoft.Xrm.Sdk.Query.QueryExpression;
            if (qe != null)
            {
                return this.Execute(qe, allEntities);
            }
            throw new Exceptions.UnsupportedTypeException($"{typeof(QueryResolver).FullName} does not support ", query.GetType());
        }

        private Microsoft.Xrm.Sdk.EntityCollection Execute(Microsoft.Xrm.Sdk.Query.QueryExpression qe, EntityShadow[] allEntities)
        {
            var baseEntities = (from a in allEntities where a.LogicalName == qe.EntityName select a).ToArray();

            var resultContainer = new QueryResultContainer(qe.ColumnSet, baseEntities, qe.EntityName);

            if (qe.Criteria != null)
            {
                resultContainer.ApplyFilter(string.Empty, qe.Criteria);
            }

            if (qe.LinkEntities != null && qe.LinkEntities.Count > 0)
            {
                throw new Exception("not supported yet");
            }

            return resultContainer.ToEntities(qe.Distinct, qe.Orders);
        }
    }
}
