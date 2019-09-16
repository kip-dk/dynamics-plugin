using System;
using System.Linq;

namespace Kipon.Dynamics.Plugin.Entities
{
    public class CrmRepository<T> : IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        private ContextService context;

        public CrmRepository(ContextService context)
        {
            this.context = context;
        }

        public IQueryable<T> GetQuery()
        {
            return context.CreateQuery<T>();
        }

        public void Delete(T entity)
        {
            this.context.DeleteObject(entity);
        }

        public void Add(T entity)
        {
            this.context.AddObject(entity);
        }

        public void Attach(T entity)
        {
            this.context.Attach(entity);
        }

        public void Detach(T entity)
        {
            this.context.Detach(entity);
        }

        public void Update(T entity)
        {
            if (!this.context.IsAttached(entity))
            {
                this.context.Attach(entity);
            }

            this.context.UpdateObject(entity);
        }

        public T[] GetAll()
        {
            return (from e in this.GetQuery() select e).ToArray();
        }

        public T GetById(Guid id)
        {
            return (from q in this.GetQuery()
                    where q.Id == id
                    select q).Single();
        }

        public T Clean(T source)
        {
            var result = new T();
            if (source.Id != Guid.Empty)
            {
                result.Id = source.Id;
            }
            else
            {
                result.Id = (Guid)source.GetType().GetProperty((source.LogicalName + "Id")).GetValue(source);
            }

            this.context.Detach(source);
            this.context.Attach(result);
            this.context.UpdateObject(result);

            return result;
        }

        public void SetStatus(Guid id, int statusCode, int stateCode)
        {
            var logicalName = (string)typeof(T).GetField("EntityLogicalName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);

            var request = new Microsoft.Crm.Sdk.Messages.SetStateRequest()
            {
                EntityMoniker = new Microsoft.Xrm.Sdk.EntityReference(logicalName, id),
                State = new Microsoft.Xrm.Sdk.OptionSetValue(stateCode),
                Status = new Microsoft.Xrm.Sdk.OptionSetValue(statusCode)
            };

            this.context.Execute(request);
        }
    }
}
