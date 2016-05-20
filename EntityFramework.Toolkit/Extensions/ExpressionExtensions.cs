using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Extensions.Extensions
{
    internal static class ExpressionExtensions
    {
        private static MemberExpression GetMemberExpression(this LambdaExpression lambdaExpression)
        {
            var member = lambdaExpression.Body as MemberExpression;
            var unary = lambdaExpression.Body as UnaryExpression;
            var memberExpression = member ?? (unary != null ? unary.Operand as MemberExpression : null);

            if (memberExpression == null)
            {
                throw new ArgumentException("'lambdaExpression' should be a member expression");
            }

            return memberExpression;
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
    }
}
