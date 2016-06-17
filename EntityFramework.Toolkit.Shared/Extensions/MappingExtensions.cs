using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace System.Data.Extensions.Extensions
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Note: If the unique property is of type string then you have to set the MaxLength.
        /// </summary>
        /// <param name="property">The property instance.</param>
        /// <param name="indexName">The name of the database index. Default is "IX_{some GUID}" if not defined.</param>
        /// <param name="columnOrder">The column order of the index. Default is 0 if not defined.</param>
        /// <returns></returns>
        public static PrimitivePropertyConfiguration IsUnique(
           this PrimitivePropertyConfiguration property,
           string indexName = "",
           int columnOrder = 0)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                indexName = "IX_" + Guid.NewGuid();
            }

            var indexAttribute = new IndexAttribute(indexName, columnOrder) { IsUnique = true };
            var indexAnnotation = new IndexAnnotation(indexAttribute);
            
            return property.HasColumnAnnotation(IndexAnnotation.AnnotationName, indexAnnotation);
        }
    }
}
