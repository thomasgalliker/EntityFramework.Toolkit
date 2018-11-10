using System.Linq.Expressions;
using EntityFramework.Toolkit.Extensions;

namespace EntityFramework.Toolkit.Utils
{
    internal static class DbHelpers
    {
        internal static bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            Expression expression1 = expression.RemoveConvert();
            MemberExpression memberExpression = expression1 as MemberExpression;
            MethodCallExpression methodCallExpression = expression1 as MethodCallExpression;
            if (memberExpression != null)
            {
                string name = memberExpression.Member.Name;
                string path1;
                if (!TryParsePath(memberExpression.Expression, out path1))
                {
                    return false;
                }
                path = path1 == null ? name : path1 + "." + name;
            }
            else if (methodCallExpression != null)
            {
                string path1;

                if (methodCallExpression.Method.Name == "Select" &&
                    methodCallExpression.Arguments.Count == 2 && 
                    TryParsePath(methodCallExpression.Arguments[0], out path1) 
                    && path1 != null)
                {
                    LambdaExpression lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
                    string path2;
                    if (lambdaExpression != null && TryParsePath(lambdaExpression.Body, out path2) && path2 != null)
                    {
                        path = path1 + "." + path2;
                        return true;
                    }
                }
                //if (methodCallExpression.Method.Name == "As" &&
                //    methodCallExpression.Arguments.Count == 1)
                //{
                //    var asType = methodCallExpression.Type;
                //    if (asType != null)
                //    {
                //        path = asType.Name;
                //        return true;
                //    }

                //    string path2;
                //    if (TryParsePath(methodCallExpression, out path2) && path2 != null)
                //    {
                //        path = path2;
                //        return true;
                //    }
                //}
                return false;
            }
            return true;
        }
    }
}