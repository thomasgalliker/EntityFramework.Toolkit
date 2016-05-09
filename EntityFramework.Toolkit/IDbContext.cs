using System.Data.Entity;

namespace System.Data.Extensions
{
    public interface IDbContext : IContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class; 
    }
}