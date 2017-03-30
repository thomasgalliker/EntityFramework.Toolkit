using System.Collections.Generic;
using System.Data.Entity.Migrations;

using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Extensions;

using ToolkitSample.DataAccess.Context;

namespace ToolkitSample.DataAccess.Migrations
{
    internal class EmployeeContextMigrationConfiguration : DbMigrationsConfiguration<EmployeeContext>
    {
        private readonly IEnumerable<IDataSeed> dataSeeds;

        public EmployeeContextMigrationConfiguration() : this(new IDataSeed[] { })
        {
        }

        public EmployeeContextMigrationConfiguration(IEnumerable<IDataSeed> dataSeeds)
        {
            this.AutomaticMigrationsEnabled = true;
            ////this.AutomaticMigrationDataLossAllowed = true; // Enable this to enforce migration with dataloss

            this.dataSeeds = dataSeeds;
        }

        protected override void Seed(EmployeeContext context)
        {
            context.Seed(this.dataSeeds);
        }
    }
}