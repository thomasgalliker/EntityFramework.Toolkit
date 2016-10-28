using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

using QueryableExtensions = EntityFramework.Toolkit.Extensions.QueryableExtensions;

namespace EntityFramework.Toolkit
{
    internal class QueryableIncluding<T> : IQueryableIncluding<T>
        where T : class
    {
        private readonly DbContext dbContext;
        private IQueryable<T> query;

        public QueryableIncluding(IQueryable<T> query, DbContext dbContext)
        {
            this.query = query;
            this.dbContext = dbContext;
        }

        public IQueryableIncluding<T> Include(Expression<Func<T, object>> includeExpression)
        {
            this.query = QueryableExtensions.Include(this.query, includeExpression);
            return this;
        }

        public IQueryableIncluding<T> Include(string includePath)
        {
            this.query = this.query.Include(includePath);
            return this;
        }

        public IQueryableIncluding<T> IncludeAllRelated()
        {
            var navigationProperties = this.dbContext.GetNavigationProperties<T>();
            foreach (var navigationProperty in navigationProperties)
            {
                this.query = this.query.Include(navigationProperty.Name);
            }

            return this;
        }

        public IQueryableIncluding<T> IncludeAllRelatedDerived()
        {
            var derivedTypes = typeof(T).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);

            var subQueries = new List<IQueryable<T>>();

            foreach (var derivedType in derivedTypes)
            {
                var navigationProperties = this.dbContext.GetNavigationProperties(derivedType);

                IQueryable<T> subquery = null;
                foreach (var navigationProperty in navigationProperties)
                {
                    if (subquery == null)
                    {
                        subquery = this.query.OfType(derivedType).Include(navigationProperty.Name).As<IQueryable<T>>();
                    }
                    else
                    {
                        subquery = subquery.Include(navigationProperty.Name).As<IQueryable<T>>();
                    }
                }
                subQueries.Add(subquery);
            }

            IEnumerable<T> result = null;
            foreach (var subQuery in subQueries)
            {
                if (result == null)
                {
                    result = subQuery.AsEnumerable();
                }
                else
                {
                    result = result.Union(subQuery.AsEnumerable());
                }
            }

            return new QueryableIncluding<T>(result.AsQueryable(), this.dbContext);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        public Expression Expression
        {
            get
            {
                return this.query.Expression;
            }
        }

        public Type ElementType
        {
            get
            {
                return this.query.ElementType;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.query.Provider;
            }
        }
    }
}