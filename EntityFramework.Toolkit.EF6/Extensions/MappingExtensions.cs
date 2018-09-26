using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.Toolkit.EF6.Utils;

namespace EntityFramework.Toolkit.EF6.Extensions
{
    public static class MappingExtensions
    {
        private static readonly Type[] TypeConfigurationNonGenericTypes = {
            typeof(DbGeometry),
            typeof(DbGeography),
            typeof(string),
            typeof(byte[]),
            typeof(decimal),
            typeof(decimal?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
        };

        /// <summary>
        ///     Marks the properties in <paramref name="propertyExpressions" /> as unique.
        ///     The index name is composed of all property names, e.g. UQ_PropertyA_PropertyB.
        /// </summary>
        /// <remarks>
        ///     Note: If the unique property is of type string, you have to set the MaxLength.
        /// </remarks>
        /// <param name="config">The affected EntityTypeConfiguration."/></param>
        /// <param name="propertyExpressions">The properties which are unique together.</param>
        public static PrimitivePropertyConfiguration[] Unique<T>(this EntityTypeConfiguration<T> config, params Expression<Func<T, object>>[] propertyExpressions) where T : class
        {
            var propertyNames = propertyExpressions.Select(pe => pe.GetPropertyInfo().Name).ToArray();

            var separatedPropertyNames = string.Join("_", propertyNames);
            var indexName = $"{IndexConstants.UniquePrefix}_{separatedPropertyNames}";

            return Unique(config, indexName: indexName, propertyExpressions: propertyExpressions);
        }

        /// <summary>
        ///     Marks the properties in <paramref name="propertyExpressions" /> as unique.
        /// </summary>
        /// <remarks>
        ///     Note: If the unique property is of type string, you have to set the MaxLength.
        /// </remarks>
        /// <param name="config">The affected EntityTypeConfiguration."/></param>
        /// <param name="indexName">The index name.</param>
        /// <param name="propertyExpressions">The properties which are unique together.</param>
        public static PrimitivePropertyConfiguration[] Unique<T>(this EntityTypeConfiguration<T> config, string indexName, params Expression<Func<T, object>>[] propertyExpressions) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "i");

            var propertyConfigurations = new List<PrimitivePropertyConfiguration>();
            for (int i = 0; i < propertyExpressions.Length; i++)
            {
                var propertyExpression = propertyExpressions[i];

                var propertyName = ReflectionHelper.GetMemberName(propertyExpression);
                var memberExpression = Expression.Property(parameter, propertyName);

                var propertyType = memberExpression.Type;
                var functionType = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
                var expressionType = typeof(Expression<>).MakeGenericType(functionType);

                MethodInfo method = null;

                if (TypeConfigurationNonGenericTypes.Contains(propertyType))
                {
                    // Get non-generic Property method
                    method = typeof(StructuralTypeConfiguration<T>).GetMethod("Property", new[] { expressionType });
                }
                else
                {
                    // Get generic Property method
                    var allMethods = typeof(StructuralTypeConfiguration<T>)
                        .GetMethods()
                        .Where(m => m.Name == "Property" && m.GetGenericArguments().Length == 1)
                        .ToList();

                    var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    method = allMethods
                        .Select(m => m.MakeGenericMethod(underlyingType))
                        .SingleOrDefault(gm =>
                        {
                            var func = gm.GetParameters()[0];
                            var expr = func.ParameterType.GetGenericArguments()[0];
                            return expr.GetGenericArguments()[1] == propertyType;
                        });
                }

                if (method == null)
                {
                    throw new InvalidOperationException($"Could not find method Property({expressionType.GetFormattedName()}");
                }

                var newPropertyExpression = Expression.Lambda(functionType, memberExpression, parameter);
                var property = method.Invoke(config, new object[] { newPropertyExpression }) as PrimitivePropertyConfiguration;
                var propertyConfiguration = property.IsUnique(indexName, columnOrder: i + 1);
                propertyConfigurations.Add(propertyConfiguration);
            }

            return propertyConfigurations.ToArray();
        }

        /// <summary>
        ///     Marks the <paramref name="property" /> as unique.
        /// </summary>
        /// <remarks>
        ///     Note: If the unique property is of type string, you have to set the MaxLength.
        /// </remarks>
        /// <param name="property">The property instance.</param>
        /// <param name="indexName">The name of the database index. Default is "UQ_Default" if not defined.</param>
        /// <param name="columnOrder">The column order of the index. Default is 0 if not defined.</param>
        /// <returns></returns>
        public static PrimitivePropertyConfiguration IsUnique(
            this PrimitivePropertyConfiguration property,
            string indexName = "",
            int columnOrder = 0)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                indexName = $"{IndexConstants.UniquePrefix}_{IndexConstants.DefaultIndexName}";
            }

            return property.HasIndex(indexName: indexName, isUnique: true, columnOrder: columnOrder);
        }

        /// <summary>
        ///     Applies an index to <paramref name="property" />.
        /// </summary>
        /// <remarks>
        ///     Note: If the indexed property is of type string, you have to set the MaxLength.
        /// </remarks>
        /// <param name="property">The property instance.</param>
        /// <param name="indexName">The name of the database index. Default is "IX_Default" if not defined.</param>
        /// <param name="isUnique">
        ///     Set this property to true to define a unique index. Set this property to false to define a
        ///     non-unique index.
        /// </param>
        /// <param name="columnOrder">The column order of the index. Default is 0 if not defined.</param>
        /// <returns></returns>
        public static PrimitivePropertyConfiguration HasIndex(
            this PrimitivePropertyConfiguration property,
            string indexName = "",
            bool isUnique = false,
            int columnOrder = 0)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                indexName = $"{IndexConstants.IndexPrefix}_{IndexConstants.DefaultIndexName}";
            }

            var indexAttribute = new IndexAttribute(indexName, columnOrder) { IsUnique = isUnique };
            var indexAnnotation = new IndexAnnotation(indexAttribute);

            return property.HasColumnAnnotation(IndexAnnotation.AnnotationName, indexAnnotation);
        }
    }
}