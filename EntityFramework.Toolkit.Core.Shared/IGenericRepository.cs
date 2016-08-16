using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core
{
    /// <summary>
    ///     Abstraction of a generic repository.
    /// </summary>
    public interface IGenericRepository<T>
    {
        IContext Context { get; }

        /// <summary>
        ///     Returns a collection of all entities in the context, or that can be queried from the
        ///     database, of given type <typeparamref name="T" />.
        /// </summary>
        /// <param name="includes">The lazy-loading includes.</param>
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

        /// <summary>
        ///     Removes the given collection of entities from the context underlying the set with each entity being put into
        ///     the Deleted state such that it will be deleted from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <returns>The collection of entities.</returns>
        /// <remarks>
        ///     Note that if <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled" /> is
        ///     set to true (which is
        ///     the default), then DetectChanges will be called once before delete any entities and will not be called
        ///     again. This means that in some situations RemoveRange may perform significantly better than calling
        ///     Remove multiple times would do.
        ///     Note that if any entity exists in the context in the Added state, then this method
        ///     will cause it to be detached from the context.  This is because an Added entity is assumed not to
        ///     exist in the database such that trying to delete it does not make sense.
        /// </remarks>
        IEnumerable<T> RemoveRange(IEnumerable<T> entities);

        void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty) where TEntity : class where TProperty : class;

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     The set of changes written to the underlying database. This can include
        ///     state entries for entities and/or relationships.
        /// </returns>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///     on the same context instance.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either before or after sending commands
        ///     to the database.
        /// </exception>
        ChangeSet Save();
    }
}