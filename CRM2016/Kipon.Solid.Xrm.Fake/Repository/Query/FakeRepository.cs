using Kipon.Fake.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    public class FakeRepository<T> : Kipon.Fake.Xrm.IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        void IRepository<T>.Add(T entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Attach(T entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Delete(T entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Detach(T entity)
        {
            throw new NotImplementedException();
        }

        T IRepository<T>.GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IRepository<T>.GetQuery()
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
