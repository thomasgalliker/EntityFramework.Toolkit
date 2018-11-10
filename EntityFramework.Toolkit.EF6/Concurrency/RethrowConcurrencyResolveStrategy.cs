using EntityFramework.Toolkit.Exceptions;

namespace EntityFramework.Toolkit.Concurrency
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
