using System.Diagnostics;

namespace System.Data.Extensions
{
    [DebuggerDisplay("Name={this.Name}, ConnectionString={this.ConnectionString}")]
    public class DbConnection : IDbConnection
    {
        public DbConnection(string connectionString) : this(string.Empty, connectionString)
        {
        }

        public DbConnection(string name, string connectionString)
        {
            this.Name = name;
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