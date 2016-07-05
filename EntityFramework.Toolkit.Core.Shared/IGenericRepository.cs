using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core
{
    public interface IGenericRepository<T>
    {
        IContext Context { get; }

        IQueryable<T> Get(params Expression<Func<T, object>>[] includes);

        IEnumerable<T> GetAll();

        /// <summary>
        /// Finds an entity with the given primary key values.
        /// If an entity with the given primary key values exists in the context, then it is
        /// returned immediately without making a request to the store.  Otherwise, a request
        /// is made to the store for an entity with the given primary key values and this entity,
        /// if found, is attached to the context and returned.  If no entity is found in the
        /// context or the store, then null is returned.
        /// </summary>
        /// <param name="ids"> The values of the primary key for the entity to be found. </param>
        /// <returns> The entity found, or null. </returns>
        T FindById(params object[] ids);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        T Add(T entity);

        IEnumerable<T> AddRange(IEnumerable<T> entity);

        T Remove(T entity);

        IEnumerable<T> RemoveAll(Expression<Func<T, bool>> predicate = null);

        IEnumerable<T> RemoveRange(IEnumerable<T> entities);

        void Edit(T entity);

        void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class;

        /// <summary>
        /// Saves the changes made in this repository.
        /// </summary>
        /// <returns></returns>
        ChangeSet Save();
    }
}