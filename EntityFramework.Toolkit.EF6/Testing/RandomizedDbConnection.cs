namespace EntityFramework.Toolkit.EF6.Testing
{
    public class RandomizedDbConnection : DbConnection
    {
        public RandomizedDbConnection(string connectionString) : base(connectionString.RandomizeDatabaseName())
        {
        }
    }
}