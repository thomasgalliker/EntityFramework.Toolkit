using System;
using System.Data.Entity;
using System.Diagnostics;
using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;

using ToolkitSample.Model;

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

        public EmployeeContext(IDbConnection dbConnection, IDatabaseInitializer<EmployeeContext> initializer, Action<string> log)
            : base(dbConnection, initializer, log)
        {
            this.Database.Log = s => Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Database.KillConnectionsToTheDatabase();
            
            modelBuilder.Configurations.Add(new PersonEntityConfiguration<Person>());
            modelBuilder.Configurations.Add(new EmployeeEntityConfiguration());
            modelBuilder.Configurations.Add(new StudentEntityConfiguration());
            modelBuilder.Configurations.Add(new DepartmentEntityConfiguration());
            modelBuilder.Configurations.Add(new CountryEntityConfiguration());
            modelBuilder.Configurations.Add(new ApplicationSettingEntityTypeConfiguration());

            //this.AutoConfigure(modelBuilder);
            //modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
        }
    }
}