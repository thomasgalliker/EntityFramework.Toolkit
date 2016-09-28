using System;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
{
    /// <summary>
    ///     Provides a template for generic seed implementors.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for which the implementor provides a seed.</typeparam>
    public abstract class DataSeedBase<TEntity> : IDataSeed
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataSeedBase{TEntity}" /> class.
        /// </summary>
        protected DataSeedBase()
        {
            this.EntityType = typeof(TEntity);
        }

        public abstract Expression<Func<TEntity, object>> AddOrUpdateExpression { get; }

        public abstract TEntity[] GetAll();

        public Type EntityType { get; private set; }

        public Expression<Func<object, object>> GetAddOrUpdateExpression()
        {
            return this.ConvertToObject(this.AddOrUpdateExpression);
        }

        private Expression<Func<object, object>> ConvertToObject<TParm, TReturn>(Expression<Func<TParm, TReturn>> input)
        {
            var parm = Expression.Parameter(typeof(object));
            var castParm = Expression.Convert(parm, typeof(TParm));
            var body = this.ReplaceExpression(input.Body, input.Parameters[0], castParm);
            body = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<object, object>>(body, parm);
        }

        private Expression ReplaceExpression(Expression body, Expression source, Expression dest)
        {
            var replacer = new ExpressionReplacer(source, dest);
            return replacer.Visit(body);
        }

        /// <summary>
        ///     Source:
        ///     http://stackoverflow.com/questions/26253321/convert-expressionfunct-tproperty-to-expressionfuncobject-object-and-v
        /// </summary>
        private class ExpressionReplacer : ExpressionVisitor
        {
            readonly Expression source;
            readonly Expression dest;

            public ExpressionReplacer(Expression source, Expression dest)
            {
                this.source = source;
                this.dest = dest;
            }

            public override Expression Visit(Expression node)
            {
                if (node == this.source)
                {
                    return this.dest;
                }

                return base.Visit(node);
            }
        }

        public object[] GetAllObjects()
        {
            return this.GetAll().Cast<object>().ToArray();
        }
    }
}