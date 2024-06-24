namespace Kipon.Xrm.Implementations
{
    using Extensions.QueryExpression;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryResolver<T> where T: Microsoft.Xrm.Sdk.Entity
    {
        private readonly ICollection<T> entities;
        private readonly QueryExpression query;
        private int totalCount = 0;
        private string[] quickFindColumns;

        public QueryResolver(ICollection<T> entities, Microsoft.Xrm.Sdk.Query.QueryExpression query, params string[] quickFindColumns)
        {
            this.entities = entities;
            this.query = query;
            this.totalCount = entities?.Count() ?? 0;
            this.quickFindColumns = quickFindColumns;
        }

        public Microsoft.Xrm.Sdk.EntityCollection Resolve()
        {
            var result = new Microsoft.Xrm.Sdk.EntityCollection();

            var include = new List<Microsoft.Xrm.Sdk.Entity>();

            var quickFind = this.query.QuickFindFilter();

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!string.IsNullOrEmpty(quickFind) && quickFindColumns != null && quickFindColumns.Length > 0 && !entity.QuickFindMatch(quickFind, quickFindColumns))
                    {
                        continue;
                    }

                    if (entity.Match(this.query.Criteria))
                    {
                        include.Add(entity);
                    }
                }
            }

            include = this.Sort(include, query);
            bool isLast = true;

            result.TotalRecordCount = include.Count;

            if (include.Count > 0)
            {
                var last = include.Last();

                if (query.PageInfo != null)
                {
                    if (query.PageInfo.PageNumber > 1)
                    {
                        include = include.Skip((query.PageInfo.PageNumber  - 1) * query.PageInfo.Count).ToList();
                    }

                    include = include.Take(query.PageInfo.Count).ToList();
                    isLast = include.Count == 0 || include.Last() == last;
                }
            }

            result.MoreRecords = !isLast;

            foreach (var r in include)
            {
                result.Entities.Add(r);
            }
            return result;
        }

        private List<Microsoft.Xrm.Sdk.Entity> Sort(List<Microsoft.Xrm.Sdk.Entity> values, Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            if (query != null && query.Orders != null && query.Orders.Count > 0)
            {
                return values.Select(r => new Sorter(r, query.Orders)).OrderBy(s => s).Select(s => s.entity).ToList();

            }
            return values;
        }
    }
}
