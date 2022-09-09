
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

        public Expression Expression 
        {
            get
            {
                return this.rootQuery.Expression;
            }
        }

        public Type ElementType 
        {
            get
            {
                return this.rootQuery.ElementType;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                this.ClearCache();
                return rootQuery.Provider;
            }
        }

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

        private void ClearCache()
        {
            var ents = this.cache.GetAttachedEntities().ToArray();
            if (ents.Length > 0)
            {
                var logicalName = new T().LogicalName;
                foreach (var c in ents)
                {
                    if (c.LogicalName == logicalName)
                    {
                        this.cache.Detach(c);
                    }
                }
            }
        }
    }
}
