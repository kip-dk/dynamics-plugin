﻿namespace Kipon.Xrm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interface for generic repository implementations.
    /// </summary>
    public interface IRepository<T> where T: Microsoft.Xrm.Sdk.Entity, new()
    {
        string LogicalName { get; }

        /// <summary>
        /// Gets an <see cref="IQueryable{T}"/> to perform further operations.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/>.</returns>
        IQueryable<T> GetQuery();

        /// <summary>
        /// Get single know instance of T with id equals id
        /// </summary>
        /// <param name="id">The unique id of the instance</param>
        /// <returns></returns>
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
    }
}
