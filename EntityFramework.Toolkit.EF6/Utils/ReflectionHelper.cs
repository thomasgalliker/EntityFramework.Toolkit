using System;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.Toolkit.EF6.Extensions;

namespace EntityFramework.Toolkit.EF6.Utils
{
    internal static class ReflectionHelper
    {
        private static MethodInfo GetGenericMethod(Expression<Action> expression, Type genericType)
        {
            var genericMethodDefinition = GetMethod(expression).GetGenericMethodDefinition();
            var methodDefinition = genericMethodDefinition.MakeGenericMethod(genericType);
            return methodDefinition;
        }

        private static MethodInfo GetGenericMethod<T>(Expression<Func<T>> expression, Type genericType)
        {
            var genericMethodDefinition = GetMethod(expression).GetGenericMethodDefinition();
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
                parameters = new object[] { };
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

        internal static void SetPropertyValue(this object sourceObject, string propertyName, object value)
        {
            sourceObject.GetType().GetProperty(propertyName).SetValue(sourceObject, value);
        }

        internal static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("The expression cannot be null.");
            }

            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                // Reference type property or field
                return memberExpression.Member.Name;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                // Reference type method
                return methodCallExpression.Method.Name;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                // Property, field of method returning value type
                return GetMemberName(unaryExpression);
            }

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                return GetMemberName(lambdaExpression.Body);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            var methodCallExpression = unaryExpression.Operand as MethodCallExpression;
            if (methodCallExpression != null)
            {
                var methodExpression = methodCallExpression;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}