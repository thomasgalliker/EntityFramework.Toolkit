using System.Data.Entity;
using System.Diagnostics;
using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeContext : DbContextBase<EmployeeContext>, IEmployeeContext
    {
        /// <summary>
        /// Empty constructor is used for code-first database migrations.
        /// </summary>
        public EmployeeContext()
        {
        }

        public EmployeeContext(IDbConnection dbConnection, IDatabaseInitializer<EmployeeContext> initializer)
            : base(dbConnection, initializer)
        {
            this.Database.Log = s => Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Database.KillConnectionsToTheDatabase();
            this.AutoConfigure(modelBuilder);
        }
    }
}