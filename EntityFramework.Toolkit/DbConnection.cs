namespace System.Data.Extensions
{
    public class DbConnection : IDbConnection
    {
        public DbConnection(string connectionString)
        {
            this.Name = string.Empty;
            this.ConnectionString = connectionString;
        }

        public virtual string Name { get; }

        public string ConnectionString { get; }

        public override string ToString()
        {
            return this.ConnectionString;
        }
    }
}