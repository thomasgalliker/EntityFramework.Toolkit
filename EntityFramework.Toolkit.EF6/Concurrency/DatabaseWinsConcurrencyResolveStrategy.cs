namespace EntityFramework.Toolkit.EF6.Concurrency
{
    public sealed class DatabaseWinsConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return databaseEntity;
        }
    }
}
