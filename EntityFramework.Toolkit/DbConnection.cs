using System.Diagnostics;
using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
{
    [DebuggerDisplay("Name={this.Name}, ConnectionString={this.ConnectionString}")]
    public class DbConnection : IDbConnection
    {
        public DbConnection(string connectionString)
            : this(name: string.Empty, connectionString: connectionString)
        {
        }

        public DbConnection(string name, string connectionString)
        {
            this.Name = name;
            this.ConnectionString = connectionString;
            this.LazyLoadingEnabled = true;
            this.ProxyCreationEnabled = true;
            this.ForceInitialize = false;
        }

        public string Name { get; }

        public string ConnectionString { get; }

        public bool LazyLoadingEnabled { get; set; }

        public bool ProxyCreationEnabled { get; set; }

        public bool ForceInitialize { get; }
    }
}