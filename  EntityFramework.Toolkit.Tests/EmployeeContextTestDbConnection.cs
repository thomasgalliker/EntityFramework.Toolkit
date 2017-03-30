using System;

using EntityFramework.Toolkit.Testing;

namespace EntityFramework.Toolkit.Tests
{
    /// <summary>
    /// This DbConnection implementation provides a ConnectionString for testing purposes.
    /// </summary>
    internal class EmployeeContextTestDbConnection : DbConnection
    {
        public EmployeeContextTestDbConnection()
            : base(name: "EntityFramework.Toolkit.Tests",
                   connectionString: @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EF.Toolkit.Tests.mdf; Integrated Security=True;".RandomizeDatabaseName())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());

            this.LazyLoadingEnabled = false;
            this.ProxyCreationEnabled = false;
        }
    }
}