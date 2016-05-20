namespace System.Data.Extensions
{
    public class DbConnection : IDbConnection
    {
        public DbConnection(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public override string ToString()
        {
            return this.ConnectionString;
        }
    }
}