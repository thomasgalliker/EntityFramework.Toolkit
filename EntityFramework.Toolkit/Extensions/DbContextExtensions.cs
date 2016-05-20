using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Extensions.Utils;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Extensions.Extensions
{
    public static class DbContextExtensions
    {
        public static void Seed(this DbContext context, IEnumerable<IDataSeed> dataSeeds)
        {
            foreach (var dataSeed in dataSeeds)
            {
                var predicate = dataSeed.GetAddOrUpdateExpression();

                ReflectionHelper.InvokeGenericMethod(
                    null,
                    () => DbContextExtensions.AddOrUpdate<object>(null, null, null),
                    dataSeed.EntityType,
                    new object[] { context, predicate, dataSeed.GetAllObjects() });
            }
        }

        public static void AddOrUpdate<TEntity>(this DbContext context, Expression<Func<object, object>> propertyExpression, params object[] entities) where TEntity : class
        {
            var set = context.Set<TEntity>();
            var parameter = Expression.Parameter(typeof(TEntity));
            foreach (var entity in entities)
            {
                string propertyName = propertyExpression.GetPropertyInfo().Name;
                object propertyValue = entity.GetPropertyValue(propertyName);

                var equalExpression = Expression.Equal(
                    Expression.Property(parameter, propertyName),
                    Expression.Constant(propertyValue));

                var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);
                TEntity existingEntity = set.SingleOrDefault(lambdaExpression);
                if (existingEntity != null)
                {
                    context.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
                else
                {
                    context.Entry(entity).State = EntityState.Added;
                }
            }
        }
    }
}
