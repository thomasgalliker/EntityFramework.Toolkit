using System.Data.Entity.Infrastructure;

namespace EntityFramework.Toolkit.EF6.Concurrency
{
    public interface IConcurrencyResolveStrategy
    {
        /// <summary>
        /// Resolves a concurrency conflict catched in <see cref="DbUpdateConcurrencyException"/>.
        /// If <code>null</code> is returned, the default strategy <see cref="RethrowConcurrencyResolveStrategy"/> is applied.
        /// </summary>
        /// <param name="conflictingEntity">The object which caused the save conflict.</param>
        /// <param name="databaseEntity">The object which is already present in the database.</param>
        /// <returns></returns>
        object ResolveConcurrencyException(object conflictingEntity, object databaseEntity);
    }
}