

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

        private Microsoft.Xrm.Sdk.Entity[] cached;
        private bool emptied = false;

        public EnumeratorWrapper(IEnumerator<T> root, ServiceAPI.IEntityCache cache)
        {
            this.root = root;
            this.cache = cache;
        }

        public T Current => GetCurrent();

        object IEnumerator.Current => GetCurrent();

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            this.Empty();

            var next = root.MoveNext();

            if (next == false)
            {
                if (cached != null && cached != null)
                {
                    foreach (var c in cached)
                    {
                        cache.Attach(c);
                    }
                }
            }
            return next;
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

        private void Empty()
        {
            if (!emptied)
            {
                var t = new T();

                this.cached = cache.GetAttachedEntities().Where(c => c.LogicalName == t.LogicalName).ToArray();
                foreach (var c in cached)
                {
                    cache.Detach(c);
                }
                this.emptied = true;
            }
        }
    }
}
