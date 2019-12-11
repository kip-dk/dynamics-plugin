using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResultContainer
    {
        private Dictionary<string, string[]> columns = new Dictionary<string, string[]>();
        private EntityContainer[] Entities;

        internal QueryResultContainer(string[] columns, Entity[] initialEntities)
        {
            this.columns.Add("", columns);
            this.Entities = initialEntities.Select(r => new EntityContainer(r)).ToArray();
        }

        internal void ApplyFilter(Microsoft.Xrm.Sdk.Query.FilterExpression criteria)
        {
            if (criteria != null && criteria.Conditions != null && criteria.Conditions.Count > 0)
            {
            }
        }
    }
}
