using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Fake.Extensions.Query;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResolver
    {
        internal Microsoft.Xrm.Sdk.EntityCollection ExecuteQuery(Microsoft.Xrm.Sdk.Query.QueryBase query, Entity[] allEntities)
        {
            var qe = query as Microsoft.Xrm.Sdk.Query.QueryExpression;
            if (qe != null)
            {
                return this.Execute(qe, allEntities);
            }
            throw new Exceptions.UnsupportedTypeException($"{typeof(QueryResolver).FullName} does not support ", query.GetType());
        }

        private Microsoft.Xrm.Sdk.EntityCollection Execute(Microsoft.Xrm.Sdk.Query.QueryExpression qe, Entity[] allEntities)
        {
            var baseQuery = (from a in allEntities where a.LogicalName == qe.EntityName select new QueryResult(a, qe.ColumnSet.ToAttributNames()));

            var r = baseQuery.ToArray();

            foreach (var l in qe.LinkEntities)
            {
                var addQuery = from a in allEntities where a.LogicalName == l.LinkToEntityName select a;

                baseQuery = (from b in r
                             join a in addQuery on b[l.LinkToEntityName, l.LinkFromAttributeName] equals a[l.LinkToAttributeName]
                             select b.Add(l.EntityAlias, a, l.Columns.ToAttributNames()));
            }

            return null;
        }
    }
}
