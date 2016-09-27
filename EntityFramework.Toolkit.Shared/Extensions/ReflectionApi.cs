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