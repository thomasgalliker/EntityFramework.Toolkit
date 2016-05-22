using System.Linq.Expressions;
#if !NET40
using System.Threading.Tasks;
#endif

namespace System.Data.Extensions
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
        int SaveChanges();

#if !NET40
        Task<int> SaveChangesAsync();
#endif
    }
}