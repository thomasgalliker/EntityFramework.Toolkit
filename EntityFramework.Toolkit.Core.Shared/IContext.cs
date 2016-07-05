using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
#if !NET40

#endif

namespace EntityFramework.Toolkit.Core
{
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Drops and recreates the underlying database.
        /// USE WITH CARE!
        /// </summary>
        void ResetDatabase();

        void Edit<TEntity>(TEntity entity) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class;

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the context has been disposed.</exception>
        ChangeSet SaveChanges();

#if !NET40
        Task<int> SaveChangesAsync();
#endif
    }
}