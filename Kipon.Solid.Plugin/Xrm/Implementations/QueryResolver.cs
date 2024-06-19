namespace Kipon.Xrm.Implementations
{
    using Kipon.Xrm.Extensions.QueryExpression;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryResolver<T> where T: Microsoft.Xrm.Sdk.Entity
    {
        private readonly ICollection<T> entities;
        private readonly QueryExpression query;
        private int totalCount = 0;

        public QueryResolver(ICollection<T> entities, Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            this.entities = entities;
            this.query = query;
            this.totalCount = entities.Count();
        }

        public Microsoft.Xrm.Sdk.EntityCollection Resolve()
        {
            var result = new Microsoft.Xrm.Sdk.EntityCollection();

            var include = new List<Microsoft.Xrm.Sdk.Entity>();

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (entity.Match(this.query.Criteria))
                    {
                        include.Add(entity);
                    }
                }
            }

            include = include.Sort(query).ToList();

            bool isLast = true;

            if (include.Count > 0)
            {
                var last = include.Last();

                if (query.PageInfo != null)
                {
                    if (query.PageInfo.PageNumber > 1)
                    {
                        include = include.Skip(query.PageInfo.PageNumber * query.PageInfo.Count).ToList();
                        include = include.Take(query.PageInfo.Count).ToList();
                        isLast = include.Count == 0 || include.Last() == last;
                    }
                }
            }

            result.MoreRecords = !isLast;
            result.TotalRecordCount = this.totalCount;

            foreach (var r in include)
            {
                result.Entities.Add(r);
            }
            return result;
        }
    }
}
