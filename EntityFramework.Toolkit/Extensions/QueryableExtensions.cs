using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Extensions.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Includes navigation properties.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="properties">A list of properties to include.</param>
        /// <returns>New queryable which includes the given navigation properties.</returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> queryable, params Expression<Func<T, object>>[] properties)
        {
            
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            foreach (Expression<Func<T, object>> property in properties)
            {
                queryable = Entity.QueryableExtensions.Include(queryable, property);
            }

            return queryable;
        }
    }
}
