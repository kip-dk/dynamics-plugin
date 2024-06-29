using Kipon.Fake.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kipon.Xrm.Fake
{
    public class Repository<T> : Kipon.Fake.Xrm.IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        public string LogicalName => new T().LogicalName;

        private Dictionary<Guid, T> data = new Dictionary<Guid, T>();

        void IRepository<T>.Add(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            data[entity.Id] = entity;
        }

        void IRepository<T>.Attach(T entity)
        {
            data[entity.Id] = entity;
        }

        void IRepository<T>.Delete(T entity)
        {
            data.Remove(entity.Id);
        }

        void IRepository<T>.Detach(T entity)
        {
            data.Remove(entity.Id);
        }

        T IRepository<T>.GetById(Guid id)
        {
            return data[id];
        }

        IQueryable<T> IRepository<T>.GetQuery()
        {
            return data.Values.AsQueryable(); 
        }

        void IRepository<T>.Update(T entity)
        {
            var me = data[entity.Id];
            foreach (var attr in entity.Attributes)
            {
                me[attr.Key] = attr.Value;
            }
        }
    }
}
