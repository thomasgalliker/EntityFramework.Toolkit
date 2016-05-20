using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Extensions.Utils
{
    internal static class ReflectionHelper
    {
        private static MethodInfo GetGenericMethod(Expression<Action> expression, Type genericType)
        {
            var genericMethodDefinition = ReflectionHelper.GetMethod(expression).GetGenericMethodDefinition();
            var methodDefinition = genericMethodDefinition.MakeGenericMethod(genericType);
            return methodDefinition;
        }

        private static MethodInfo GetGenericMethod<T>(Expression<Func<T>> expression, Type genericType)
        {
            var genericMethodDefinition = ReflectionHelper.GetMethod(expression).GetGenericMethodDefinition();
            var methodDefinition = genericMethodDefinition.MakeGenericMethod(genericType);
            return methodDefinition;
        }

        internal static object InvokeGenericMethod(object target, Expression<Action> expression, Type genericType, object[] parameters = null)
        {
            if (parameters == null)
            {
                parameters = new object[] { };
            }

            var result = GetGenericMethod(expression, genericType).Invoke(target, parameters);

            return result;
        }

        internal static object InvokeGenericMethod<T>(object target, Expression<Func<T>> expression, Type genericType, object[] parameters = null)
        {
            if (parameters == null)
            {
                parameters = new object[] {};
            }

            var result = GetGenericMethod(expression, genericType).Invoke(target, parameters);

            return result;
        }

        private static MethodInfo GetMethod(Expression<Action> expression)
        {
            MethodCallExpression callExpression = (MethodCallExpression)expression.Body;
            return callExpression.Method;
        }

        private static MethodInfo GetMethod<T>(Expression<Func<T>> expression)
        {
            MethodCallExpression callExpression = (MethodCallExpression)expression.Body;
            return callExpression.Method;
        }

        internal static object GetPropertyValue(this object sourceObject, string propertyName)
        {
            return sourceObject.GetType().GetProperty(propertyName).GetValue(sourceObject, null);
        }
    }
}
