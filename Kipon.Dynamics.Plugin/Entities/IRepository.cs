using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Entities
{
    /// <summary>
    /// Interface for generic repository implementations.
    /// </summary>
    /// <typeparam name="T">Class that implement <see cref="Portal.Core.Model.IEntity"/>.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets an <see cref="IQueryable{T}"/> to perform further operations.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/>.</returns>
        IQueryable<T> GetQuery();

        /// <summary>
        /// Gets all records.
        /// </summary>
        T[] GetAll();

        T GetById(Guid id);

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Adds the given entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Updates the given entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Attaches the the given entity to the current context.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        void Attach(T entity);


        /// <summary>
        /// Detach the object from the context
        /// </summary>
        /// <param name="entity"></param>
        void Detach(T entity);


        /// <summary>
        /// Detach source from context, create a new instance of same type and same Id, and attach that to the context.
        /// The resulting record has the advantage that it only saves properties that was actually changed after call to the Mirror function
        /// </summary>
        T Clean(T entity);

        /// <summary>
        /// Sets the status code on the entity with the given id.
        /// </summary>
        void SetStatus(Guid id, int statusCode, int stateCode);
    }
}
