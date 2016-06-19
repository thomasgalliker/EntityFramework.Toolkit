using System.Diagnostics;

using EntityFramework.Toolkit.Core;

namespace EntityFramework.Toolkit
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
            this.LazyLoadingEnabled = true;
            this.ProxyCreationEnabled = true;
        }

        public virtual string Name { get; }

        public string ConnectionString { get; }

        public virtual bool LazyLoadingEnabled { get; set; }

        public virtual bool ProxyCreationEnabled { get; set; }

        public override string ToString()
        {
            return this.ConnectionString;
        }
    }
}