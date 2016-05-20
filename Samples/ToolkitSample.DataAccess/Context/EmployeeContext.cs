using System.Data.Entity;
using System.Data.Extensions;
using System.Data.Extensions.Extensions;
using System.Diagnostics;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeContext : DbContextBase<EmployeeContext>, IEmployeeContext
    {
        public EmployeeContext(IDbConnection dbConnection, IDatabaseInitializer<EmployeeContext> initializer)
            : base(dbConnection, initializer)
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