using EntityFramework.Toolkit.EF6.Exceptions;

namespace EntityFramework.Toolkit.EF6.Concurrency
{
    /// <summary>
    /// Rethrow strategy throws an <see cref="UpdateConcurrencyException"/> in case of a conflicting update.
    /// </summary>
    public sealed class RethrowConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return null;
        }
    }
}
