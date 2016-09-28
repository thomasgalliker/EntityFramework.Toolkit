using System.Data.Entity;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
{
    public interface IDbContext : IContext
    {
        string Name { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}