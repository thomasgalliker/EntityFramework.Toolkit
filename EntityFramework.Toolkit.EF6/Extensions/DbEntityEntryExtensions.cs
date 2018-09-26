using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace EntityFramework.Toolkit.EF6.Extensions
{
    public static class DbEntityEntryExtensions
    {
        public static IEnumerable<DbPropertyEntry> GetProperties(this DbEntityEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            foreach (string modifiedProperty in entry.CurrentValues.PropertyNames)
            {
                var property = entry.Property(modifiedProperty);
                yield return property;
            }
        }

        /// <summary>
        /// Gets an object that represents a scalar or complex property of this entity.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="property">The property.</param>
        /// <returns> An object representing the property. </returns>
        public static DbPropertyEntry Property<T>(this DbEntityEntry entry, Expression<Func<T, object>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyInfo = property.GetPropertyInfo();
            return entry.Property(propertyInfo.Name);
        }
    }
}
