using System;
using EntityFramework.Toolkit;

namespace ToolkitSample.DataAccess.Context
{
    /// <summary>
    /// This DbConnection implementation provides a ConnectionString for production.
    /// You can receive the production ConnectionString from an application configuration (app.config) if you like.
    /// </summary>
    internal class EmployeeContextDbConnection : DbConnection
    {
        public EmployeeContextDbConnection()
            : base(name: "EntityFramework.Toolkit", 
                   connectionString: @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EntityFramework.Toolkit.mdf; Integrated Security=True;")
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }
    }
}