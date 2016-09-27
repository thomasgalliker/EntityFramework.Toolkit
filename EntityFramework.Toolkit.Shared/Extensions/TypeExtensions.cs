using System;
using System.Linq;

namespace CrossPlatformLibrary.Extensions
{
    internal static class TypeExtensions
    {
        internal static string GetFormattedName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsGenericType())
            {
                return type.Name;
            }

            return $"{type.Name.Substring(0, type.Name.IndexOf('`'))}<{string.Join(", ", type.GetGenericArguments().Select(t => t.GetFormattedName()))}>";
        }

        internal static string GetFormattedFullname(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsGenericType())
            {
                return type.ToString();
            }

            return $"{type.Namespace}.{type.Name.Substring(0, type.Name.IndexOf('`'))}<{string.Join(", ", type.GetGenericArguments().Select(t => t.GetFormattedFullname()))}>";
        }
    }
}