using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace EntityFramework.Toolkit
{
    internal class QueryableIncluding<T> : IQueryableIncluding<T>
    {
        private IQueryable<T> query;

        public QueryableIncluding(IQueryable<T> query)
        {
            this.query = query;
        }

        public IQueryableIncluding<T> Include(Expression<Func<T, object>> includeExpression)
        {
            this.query = this.query.Include(includeExpression);
            return this;
        }

        public IQueryableIncluding<T> Include(string includePath)
        {
            this.query = QueryableExtensions.Include(this.query, includePath);
            return this;
        }

        public IQueryableIncluding<T> IncludeAllRelated()
        {
            throw new NotImplementedException();
            //this.query = QueryableExtensions.Include(this.query, includePath);
            //return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        public Expression Expression { get { return this.query.Expression; } }

        public Type ElementType { get { return this.query.ElementType; } }

        public IQueryProvider Provider { get { return this.query.Provider; } }
    }
}