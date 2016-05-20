using System;
using System.Data.Extensions;

namespace EntityFramework.Toolkit.Tests
{
    /// <summary>
    /// This DbConnection implementation provides a ConnectionString for testing purposes.
    /// </summary>
    public class EmployeeContextTestDbConnection : IDbConnection
    {
        public EmployeeContextTestDbConnection()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }

        public string ConnectionString
        {
            get
            {
                return @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EntityFramework.Toolkit.Tests.mdf; Integrated Security=True;";
            }
        }
    }
}