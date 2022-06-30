

namespace Kipon.Xrm.Implementations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class EnumeratorWrapper<T> : IEnumerator<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        private readonly IEnumerator<T> root;
        private readonly ServiceAPI.IEntityCache cache;

        public EnumeratorWrapper(IEnumerator<T> root, ServiceAPI.IEntityCache cache)
        {
            this.root = root;
            this.cache = cache;
        }

        public T Current => GetCurrent();

        object IEnumerator.Current => GetCurrent();

        public void Dispose()
        {
            root.Dispose();
        }

        public bool MoveNext()
        {
            return root.MoveNext();
        }

        public void Reset()
        {
            root.Reset();
        }

        private T GetCurrent()
        {
            var result = root.Current;
            cache.Detach(result);
            return result;
        }
    }
}
