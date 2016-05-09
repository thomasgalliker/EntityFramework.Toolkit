using System.Linq.Expressions;

namespace System.Data.Extensions
{
    public interface IContext : IDisposable
    {
        void Edit<TEntity>(TEntity entity) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void LoadReferenced<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class;

        int SaveChanges();
    }
}