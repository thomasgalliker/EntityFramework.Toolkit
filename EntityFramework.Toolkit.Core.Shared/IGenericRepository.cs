using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core
{
    /// <summary>
    ///     Abstraction of a generic repository.
    /// </summary>
    public interface IGenericRepository<T> : IWritableRepository, IReadOnlyRepository<T>
    {
        IContext Context { get; }

        /// <summary>
        ///     Adds the given entity to the context underlying the set in the Added state such that it will
        ///     be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns> The entity.</returns>
        /// <remarks>
        ///     Note that entities that are already in the context in some other state will have their state set
        ///     to Added. Add is a no-op if the entity is already in the context in the Added state.
        /// </remarks>
        T Add(T entity);

        TDerived Add<TDerived>(TDerived entity) where TDerived : class, T;

        IEnumerable<T> AddRange(IEnumerable<T> entity);

        /// <summary>
        ///     Adds or updates the given entity. If the entity is existing, it's going to be updated with the new values.
        ///     If the entity does not exist in the context, it's going to be created.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T AddOrUpdate(T entity);

        /// <summary>
        ///     Updates the given entity. This method checks if an entity exists before it tries to perform the update activity.
        /// </summary>
        /// <param name="entity">The entity to be updated in the database context.</param>
        void Update(T entity);

        /// <summary>
        ///     Marks the given entity as Deleted such that it will be deleted from the database when SaveChanges
        ///     is called.  Note that the entity must exist in the context in some other state before this method
        ///     is called.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns> The entity that has been removed.</returns>
        /// <remarks>
        ///     Note that if the entity exists in the context in the Added state, then this method
        ///     will cause it to be detached from the context.  This is because an Added entity is assumed not to
        ///     exist in the database such that trying to delete it does not make sense.
        /// </remarks>
        T Remove(T entity);

        void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty) where TEntity : class where TProperty : class;
    }
}