using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Entities
{
    /// <summary>
    /// Implementation of the <see cref="Larsen.Xrm.Entities.IRepository{T}"/> using the against a CRM context.
    /// </summary>
    /// <typeparam name="T">The <see cref="Portal.Core.Model.IEntity"/>.</typeparam>
    public class CrmRepository<T> : IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        /// <summary>
        /// The context.
        /// </summary>
        private ContextService context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFRepository&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CrmRepository(ContextService context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets an <see cref="IQueryable{T}"/> to perform further operations.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> GetQuery()
        {
            return context.CreateQuery<T>();
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(T entity)
        {
            this.context.DeleteObject(entity);
        }

        /// <summary>
        /// Adds the given entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(T entity)
        {
            this.context.AddObject(entity);
        }

        /// <summary>
        /// Attaches the the given entity to the current context.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        public void Attach(T entity)
        {
            this.context.Attach(entity);
        }

        public void Detach(T entity)
        {
            this.context.Detach(entity);
        }


        /// <summary>
        /// Updtes the the given entity.
        /// </summary>
        /// <param name="entity">The entity to upate.</param>
        public void Update(T entity)
        {
            if (!this.context.IsAttached(entity))
            {
                this.context.Attach(entity);
            }

            this.context.UpdateObject(entity);
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        public T[] GetAll()
        {
            return (from e in this.GetQuery() select e).ToArray();
        }

        /// <summary>
        /// Gets a record by it's primary key.
        /// </summary>
        /// <param name="id">Primary key of the reocrd.</param>
        public T GetById(Guid id)
        {
            return (from q in this.GetQuery()
                    where q.Id == id
                    select q).Single();
        }

        /// <summary>
        /// Detach source from context, create a new instance of same type and same Id, and attach that to the context.
        /// The resulting record has the advantage that it only saves properties that was actually changed after call to the Mirror function
        /// </summary>
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

        /// <summary>
        /// Sets the status code on the entity with the given id. 
        /// </summary>
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
