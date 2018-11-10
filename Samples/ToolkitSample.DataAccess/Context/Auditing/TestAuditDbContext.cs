using System;
using System.Data.Entity;
using EntityFramework.Toolkit.Auditing;
using EntityFramework.Toolkit.Core;
using ToolkitSample.Model;
using ToolkitSample.Model.Auditing;

namespace ToolkitSample.DataAccess.Context.Auditing
{
    /// <summary>
    /// This data context is used to demonstrate the auditing features.
    /// It is configured using the app.config.
    /// </summary>
    public class TestAuditDbContext : AuditDbContextBase<TestAuditDbContext>
    {
        public IDbSet<TestEntity> TestEntities { get; set; }

        public IDbSet<TestEntityAudit> TestEntityAudits { get; set; }

        public IDbSet<Employee> Employees { get; set; }

        public IDbSet<EmployeeAudit> EmployeeAudits { get; set; }

        public TestAuditDbContext(string nameOrConnectionString, IDatabaseInitializer<TestAuditDbContext> databaseInitializer)
            : base(nameOrConnectionString, databaseInitializer)
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.ConfigureAuditingFromAppConfig();
        }

        public TestAuditDbContext(string nameOrConnectionString, IDatabaseInitializer<TestAuditDbContext> databaseInitializer, Action<string> log)
            : base(nameOrConnectionString, databaseInitializer, log)
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.ConfigureAuditingFromAppConfig();
        }

        public TestAuditDbContext(IDbConnection dbConnection, IDatabaseInitializer<TestAuditDbContext> databaseInitializer, Action<string> log = null)
            : base(dbConnection, databaseInitializer, log)
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.ConfigureAuditingFromAppConfig();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new PersonEntityConfiguration<Person>());
            modelBuilder.Configurations.Add(new EmployeeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeAuditEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TestEntityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TestEntityAuditEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new StudentEntityConfiguration());
            modelBuilder.Configurations.Add(new DepartmentEntityConfiguration());
            modelBuilder.Configurations.Add(new RoomConfiguration());
            modelBuilder.Configurations.Add(new CountryEntityConfiguration());
            modelBuilder.Configurations.Add(new ApplicationSettingEntityTypeConfiguration());
        }
    }
}