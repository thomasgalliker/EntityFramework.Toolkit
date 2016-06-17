using System.Data.Entity;

namespace System.Data.Extensions
{
    public interface IDbContext : IContext
    {
        string Name { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class; 
    }
}