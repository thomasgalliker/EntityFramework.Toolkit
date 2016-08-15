using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Utils;

namespace EntityFramework.Toolkit.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        ///     Includes navigation properties.
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
                queryable = queryable.Include(property);
            }

            return queryable;
        }

        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> queriable, Expression<Func<T, TProperty>> pathExpression)
        {
            if (queriable == null)
            {
                throw new ArgumentNullException(nameof(queriable));
            }

            if (pathExpression == null)
            {
                throw new ArgumentNullException(nameof(pathExpression));
            }

            string path;

            if (!DbHelpers.TryParsePath(pathExpression.Body, out path) || path == null)
            {
                throw new ArgumentException("InvalidIncludePathExpression", nameof(pathExpression));
            }

            return queriable.Include(path);
        }
    }
}