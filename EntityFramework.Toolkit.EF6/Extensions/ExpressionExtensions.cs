using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.Toolkit.Extensions
{
    internal static class ExpressionExtensions
    {
        private static MemberExpression GetMemberExpression(this LambdaExpression lambdaExpression)
        {
            var memberExpression = lambdaExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression;
            }

            var unaryExpression = lambdaExpression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                var innerUnaryExpression = unaryExpression.Operand as UnaryExpression;
                if (innerUnaryExpression != null)
                {
                    return innerUnaryExpression.Operand as MemberExpression;
                }

                return unaryExpression.Operand as MemberExpression;
            }

            throw new ArgumentException("'lambdaExpression' should be a member expression");
        }

        internal static PropertyInfo GetPropertyInfo(this LambdaExpression lambdaExpression)
        {
            var memberExpression = GetMemberExpression(lambdaExpression);

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("'lambdaExpression' should be a property");
            }

            return propertyInfo;
        }

        internal static Expression RemoveConvert(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }
    }
}