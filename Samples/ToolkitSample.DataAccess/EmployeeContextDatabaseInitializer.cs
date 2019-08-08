using System.Data.Entity;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Migrations;

namespace ToolkitSample.DataAccess
{
    internal sealed class EmployeeContextDatabaseInitializer : MigrateDatabaseToLatestVersion<EmployeeContext, EmployeeContextMigrationConfiguration>
    {
        public EmployeeContextDatabaseInitializer()
        {
        }

        public EmployeeContextDatabaseInitializer(EmployeeContextMigrationConfiguration migrationConfiguration) 
            : base(useSuppliedContext: true, configuration: migrationConfiguration)
        {
        }
    }
}