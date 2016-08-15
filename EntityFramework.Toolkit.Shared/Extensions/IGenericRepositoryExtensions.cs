using System;
using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit.Extensions
{
    public static class IGenericRepositoryExtensions
    {
        /// <summary>
        ///     Marks the the entity with the given primary key as Deleted such that it will be deleted from the database when SaveChanges
        ///     is called. Note that the entity must exist in the context in some other state before this method
        ///     is called.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns> The entity.</returns>
        /// <remarks>
        ///     Note that if the entity exists in the context in the Added state, then this method
        ///     will cause it to be detached from the context.  This is because an Added entity is assumed not to
        ///     exist in the database such that trying to delete it does not make sense.
        /// </remarks>
        public static T RemoveById<T>(this IGenericRepository<T> repository, params object[] ids)
        {
            var entity = repository.FindById(ids);
            if (entity == null)
            {
                throw new ArgumentOutOfRangeException(nameof(ids));
            }

            return repository.Remove(entity);
        }
    }
}
