using System;
using System.Data.Extensions;

namespace EntityFramework.Toolkit.Tests
{
    /// <summary>
    /// This DbConnection implementation provides a ConnectionString for testing purposes.
    /// </summary>
    internal class EmployeeContextTestDbConnection : DbConnection
    {
        public EmployeeContextTestDbConnection()
            : base(name: "EntityFramework.Toolkit.Tests",
                   connectionString: @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EntityFramework.Toolkit.Tests.mdf; Integrated Security=True;")
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }
    }
}