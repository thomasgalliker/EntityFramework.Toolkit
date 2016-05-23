using System;
using System.Data.Extensions;

namespace EntityFramework.Toolkit.Tests
{
    /// <summary>
    /// This DbConnection implementation provides a ConnectionString for testing purposes.
    /// </summary>
    internal class EmployeeContextTestDbConnection : IDbConnection
    {
        public EmployeeContextTestDbConnection()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }

        public string Name { get { return "EntityFramework.Toolkit.Tests"; } }

        public string ConnectionString
        {
            get
            {
                return @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\" + this.Name + ".mdf; Integrated Security=True;";
            }
        }
    }
}