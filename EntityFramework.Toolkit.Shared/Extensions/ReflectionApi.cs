using System.Reflection;

#if NET40

namespace System
{
    internal static class TypeExtensions
    {
        internal static bool IsGenericType(this Type t)
        {
            return t.IsGenericType;
        }
    }

    internal static class PropertyInfoExtensions
    {
        internal static void SetValue(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value, null);
        }
    }
}

#else

using System.Reflection;

namespace System
{
    internal static class TypeExtensions
    {
        internal static bool IsGenericType(this Type t)
        {
            return t.GetTypeInfo().IsGenericType;
        }
    }
}

#endif