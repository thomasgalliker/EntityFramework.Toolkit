using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.Core
{
    public interface IQueryableIncluding<T> : IQueryable<T>
    {
        IQueryableIncluding<T> Include(string includePath);

        IQueryableIncluding<T> Include(Expression<Func<T, object>> includExpression);
    }
}