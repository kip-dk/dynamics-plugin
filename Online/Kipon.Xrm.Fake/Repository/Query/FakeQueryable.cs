using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    public class FakeQueryable<T> : IQueryable<T>, IEnumerator<T>
    {
        private IQueryable<T> source;

        public FakeQueryable(IQueryable<T> source)
        {
            this.source = source;
        }

        public Expression Expression => source.Expression;

        public Type ElementType => source.ElementType;

        public IQueryProvider Provider => source.Provider;

        public T Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }
    }
}
