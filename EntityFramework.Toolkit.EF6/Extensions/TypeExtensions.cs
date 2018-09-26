using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityFramework.Toolkit.EF6.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        ///     Gets the raw entity type without dynamic proxy type.
        /// </summary>
        public static Type GetEntityType(this DbEntityEntry entry)
        {
            var entityType = entry.Entity.GetType();
            if (entityType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                entityType = entityType.BaseType;
            }

            return entityType;
        }

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

        private static bool HasDefaultValue(this ParameterInfo parameterInfo)
        {
#if NET40
            var defaultValue = parameterInfo.RawDefaultValue;
            return (defaultValue is DBNull) == false;
#else
            return parameterInfo.HasDefaultValue;
#endif
        }

        /// <summary>
        ///     Finds the best matching constructor for given type <paramref name="type" />.
        /// </summary>
        internal static ConstructorInfoAndParameters GetMatchingConstructor(this Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var allMatched = false;
                var ctorParameters = constructor.GetParameters();
                if (args.Length >= ctorParameters.Count(p => p.HasDefaultValue() == false) && args.Length <= ctorParameters.Length)
                {
                    for (var ctorParameterIndex = 0; ctorParameterIndex < ctorParameters.Length; ctorParameterIndex++)
                    {
                        var ctorParameter = ctorParameters[ctorParameterIndex];
                        if (ctorParameterIndex >= args.Length)
                        {
                            var argsList = args.ToList();
                            argsList.Add(ctorParameter.DefaultValue);
                            args = argsList.ToArray();
                        }

                        var arg = args[ctorParameterIndex];
                        if (arg != null)
                        {
                            var argType = arg.GetType();
                            var isEqualType = argType == ctorParameter.ParameterType;


                            var interfaces = ctorParameter.ParameterType.GetInterfaces();
                            var isAnyAssignable = interfaces.Any(i => i.IsAssignableFrom(argType));
                            var isInstanceOfType = ctorParameter.ParameterType.IsInstanceOfType(arg);
                            if (isEqualType || isAnyAssignable || isInstanceOfType)
                            {
                                allMatched = true;
                            }
                            else
                            {
                                allMatched = false;
                                break;
                            }
                        }
                        else
                        {
                            if (!ctorParameter.ParameterType.IsValueType)
                            {
                                allMatched = true;
                            }
                            else
                            {
                                allMatched = false;
                                break;
                            }
                        }
                    }
                }

                if (allMatched)
                {
                    return new ConstructorInfoAndParameters(constructor, args);
                }
            }

            var typeName = type.GetFormattedName();
            var exceptionStringBuilder = new StringBuilder();
            var argTypes = args.Where(d => d != null).Select(d => d.GetType()).ToArray();
            var definedParameters = string.Join(", ", argTypes.Select(p => p.GetFormattedName()));
            exceptionStringBuilder.AppendLine(
                definedParameters.Length == 0
                    ? $"{typeName} does not have a constructor with no parameters."
                    : $"{typeName} does not have a constructor with parameter{(definedParameters.Length > 1 ? "s" : "")} ({definedParameters}).");

            if (constructors.Any())
            {
                exceptionStringBuilder.AppendLine();
                exceptionStringBuilder.AppendLine("Use one of the following constructors:");
                foreach (var constructor in constructors)
                {
                    var parameters = $"{string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))}";
                    exceptionStringBuilder.AppendLine($"{typeName}({parameters})");
                }
            }

            var exceptionMessage = exceptionStringBuilder.ToString();
            throw new InvalidOperationException(exceptionMessage);
        }
    }
}