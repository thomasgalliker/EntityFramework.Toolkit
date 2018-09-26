using System.Data.Entity;
using EntityFramework.Toolkit.EF6.Contracts;

namespace EntityFramework.Toolkit.EF6
{
    public interface IDbContext : IContext
    {
        /// <summary>
        /// The name of this EntityFramework context.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The generic DbSet of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}