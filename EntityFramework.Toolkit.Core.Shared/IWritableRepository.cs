#if !NET40
using System.Threading.Tasks;
#endif

namespace EntityFramework.Toolkit
{
    public interface IWritableRepository : IRepository
    {
        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     The set of changes written to the underlying database. This can include
        ///     state entries for entities and/or relationships.
        /// </returns>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///     on the same context instance.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either before or after sending commands
        ///     to the database.
        /// </exception>
        ChangeSet Save();

#if !NET40
        Task<ChangeSet> SaveAsync();
#endif
    }
}