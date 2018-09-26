using System;
using System.Data.Entity;
using EntityFramework.Toolkit.EF6.Auditing;
using EntityFramework.Toolkit.EF6.Contracts;
using EntityFramework.Toolkit.EF6.Extensions;
using ToolkitSample.Model;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeContext : AuditDbContextBase<EmployeeContext>, IEmployeeContext
    {
        private static readonly AuditDbContextConfiguration AuditDbContextConfiguration = new AuditDbContextConfiguration(auditEnabled: true, auditDateTimeKind: DateTimeKind.Utc);

        /// <summary>
        ///     Empty constructor is used for 'update-database' command-line command.
        /// </summary>
        public EmployeeContext()
        {
        }

        public EmployeeContext(IDbConnection dbConnection, IDatabaseInitializer<EmployeeContext> initializer)
          : base(dbConnection, initializer, null)
        {
            this.ConfigureAuditing(AuditDbContextConfiguration);
        }

        public EmployeeContext(IDbConnection dbConnection, Action<string> log)
           : base(dbConnection, null, log)
        {
            this.ConfigureAuditing(AuditDbContextConfiguration);
        }

        public EmployeeContext(IDbConnection dbConnection, IDatabaseInitializer<EmployeeContext> initializer, Action<string> log = null)
           : base(dbConnection, initializer, log)
        {
            this.ConfigureAuditing(AuditDbContextConfiguration);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Database.KillConnectionsToTheDatabase();

            modelBuilder.Configurations.Add(new PersonEntityConfiguration<Person>());
            modelBuilder.Configurations.Add(new EmployeeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeAuditEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new StudentEntityConfiguration());
            modelBuilder.Configurations.Add(new DepartmentEntityConfiguration());
            modelBuilder.Configurations.Add(new RoomConfiguration());
            modelBuilder.Configurations.Add(new CountryEntityConfiguration());
            modelBuilder.Configurations.Add(new ApplicationSettingEntityTypeConfiguration());

            //this.AutoConfigure(modelBuilder);
            //modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
        }
    }
}