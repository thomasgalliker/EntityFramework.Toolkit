using System;
using System.Linq;

namespace EntityFramework.Toolkit
{
    internal static class TypeExtensions
    {
        /// <summary>
        ///     Safely casts the specified object to the type specified through <typeparamref name="TTo" />.
        /// </summary>
        /// <remarks>
        ///     Has been introduced to allow casting objects without breaking the fluent API.
        /// </remarks>
        /// <typeparam name="TTo"></typeparam>
        public static TTo As<TTo>(this object subject)
        {
            if (subject is TTo)
            {
                return (TTo)subject;
            }
            return default(TTo);
        }

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