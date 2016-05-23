using System.Data.Entity;

namespace System.Data.Extensions
{
    public interface IDbContext : IContext
    {
        string Name { get; }

        IDbSet<TEntity> Set<TEntity>() where TEntity : class; 
    }
}