using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core.Extensions
{
    public static class ReadOnlyRepositoryExtensions
    {
        /// <summary>
        ///     Indicates whether an entity with the given primary key value exists.
        /// </summary>
        /// <param name="ids">The primary keys of the entity to be found.</param>
        /// <returns>true, if an entity with given primary key exists; otherwise, false.</returns>
        public static bool Any<T>(this IReadOnlyRepository<T> repository, params object[] ids)
        {
            return repository.FindById(ids) != null;
        }

        /// <summary>
        ///     Indicates whether an entity which matches the given predicate exists.
        /// </summary>
        /// <param name="predicate">The predicate to filter the entity.</param>
        /// <returns>true, if an entity exists for given predicate; otherwise, false.</returns>
        public static bool Any<T>(this IReadOnlyRepository<T> repository, Expression<Func<T, bool>> predicate)
        {
            return repository.Get().Any(predicate);
        }
    }
}