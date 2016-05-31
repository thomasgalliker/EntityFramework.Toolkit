using System.Data.Extensions.Exceptions;

namespace System.Data.Extensions.Concurrency
{
    /// <summary>
    /// Rethrow strategy throws an <see cref="UpdateConcurrencyException"/> in case of a conflicting update.
    /// </summary>
    public class RethrowConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return null;
        }
    }
}
