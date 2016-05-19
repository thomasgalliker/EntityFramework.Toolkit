using System.Data.Entity;
using System.Data.Extensions;
using System.Diagnostics;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeContext : DbContextBase<EmployeeContext>, IEmployeeContext
    {
        public EmployeeContext(IDatabaseInitializer<EmployeeContext> initializer = null) 
            : base(connectionString: @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EntityFramework.Toolkit.Tests.mdf; Integrated Security=True;", databaseInitializer: initializer)
        {
            this.Database.Log = s => Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Database.KillConnectionsToTheDatabase();

            modelBuilder.Configurations.Add(new EmployeeEntityConfiguration());
            modelBuilder.Configurations.Add(new DepartmentEntityConfiguration());
        }
    }
}