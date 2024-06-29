namespace Kipon.Xrm.Fake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class is to be used for unit test only. For real plugin the CrmRepository implementation will be used.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        public string LogicalName => new T().LogicalName; 

        private List<T> elements = new List<T>();
        public void Add(T entity)
        {
            elements.Add(entity);
        }

        public void Attach(T entity)
        {
            elements.Add(entity);
        }

        public void Delete(T entity)
        {
            elements.Remove(entity);
        }

        public void Detach(T entity)
        {
            elements.Remove(entity);
        }

        public T GetById(Guid id)
        {
            return elements.Where(r => r.Id == id).Single();
        }

        public IQueryable<T> GetQuery()
        {
            return this.elements.AsQueryable();
        }

        public void Update(T entity)
        {
            var me = this.elements.Where(r => r.Id == entity.Id).Single();
            foreach (var at in entity.Attributes.Keys)
            {
                me[at] = entity[at];
            }
        }
    }
}
