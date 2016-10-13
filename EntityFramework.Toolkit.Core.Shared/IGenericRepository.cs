using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core
{
    /// <summary>
    ///     Abstraction of a generic repository.
    /// </summary>
    public interface IGenericRepository<T> : IRepository
    {
        IContext Context { get; }

        /// <summary>
        ///     Returns a collection of all entities in the context, or that can be queried from the
        ///     database, of given type <typeparamref name="T" />.
        /// </summary>
        IQueryableIncluding<T> Get();

        [Obsolete("Use GenericRepository.Get().Include(...) instead")]
        IQueryable<T> Get(params Expression<Func<T, object>>[] includes);

        /// <summary>
        ///     Returns a collection of all entities in the context, or that can be queried from the
        ///     database, of given type <typeparamref name="T" />.
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        ///     Finds an entity with the given primary key values.
        ///     If an entity with the given primary key values exists in the context, then it is
        ///     returned immediately without making a request to the store.  Otherwise, a request
        ///     is made to the store for an entity with the given primary key values and this entity,
        ///     if found, is attached to the context and returned.  If no entity is found in the
        ///     context or the store, then null is returned.
        /// </summary>
        /// <param name="ids">The values of the primary key for the entity to be found. </param>
        /// <returns> The entity found, or null. </returns>
        T FindById(params object[] ids);

        /// <summary>
        ///     Finds entities with the given search predicate.
        /// </summary>
        /// <param name="predicate">The search predicate.</param>
        /// <returns>A collection of entities matching the search predicate.</returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

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