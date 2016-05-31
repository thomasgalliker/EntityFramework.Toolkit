namespace System.Data.Extensions.Concurrency
{
    public class DatabaseWinsConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            return databaseEntity;
        }
    }
}
