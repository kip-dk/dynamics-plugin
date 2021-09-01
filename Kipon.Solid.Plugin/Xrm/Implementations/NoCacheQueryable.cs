
namespace Kipon.Xrm.Implementations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public class NoCacheQueryable<T> : IQueryable<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        private readonly IQueryable<T> rootQuery;
        private readonly ServiceAPI.IEntityCache cache;
        private IEnumerator<T> rootEnum;

        public NoCacheQueryable(IQueryable<T> rootQuery, ServiceAPI.IEntityCache cache)
        {
            this.rootQuery = rootQuery;
            this.cache = cache;
        }

        public Expression Expression => this.rootQuery.Expression;

        public Type ElementType => this.rootQuery.ElementType;

        public IQueryProvider Provider => this.rootQuery.Provider;

        public IEnumerator<T> GetEnumerator()
        {
            return this.GetE();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetE();
        }

        private IEnumerator<T> GetE()
        {
            if (rootEnum == null)
            {
                rootEnum = new EnumeratorWrapper<T>(rootQuery.GetEnumerator(), this.cache);
            }
            return rootEnum;
        }
    }
}
