namespace System.Data.Extensions.Concurrency
{
    public sealed class DatabaseWinsConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return databaseEntity;
        }
    }
}
