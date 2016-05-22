using System.Data.Extensions;
using System.Data.Extensions.Testing;
using System.Linq;

using FluentAssertions;

using ToolkitSample.DataAccess;
using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Migrations;
using ToolkitSample.DataAccess.Model;
using ToolkitSample.DataAccess.Seed;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class DataSeedTests_IntegrationTests : ContextTestBase<EmployeeContext>
    {
        public DataSeedTests_IntegrationTests()
            : base(dbConnection: new EmployeeContextTestDbConnection(),
                     initializeDatabase: false)
        {
        }

        [Fact]
        public void ShouldInitializeContextWithEmptyDataSeed()
        {
            // Arrange
            var emptyDataSeed = new IDataSeed[] { };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(emptyDataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            // Assert
            var allDepartments = this.Context.Set<Department>().ToList();
            allDepartments.Should().HaveCount(0);
        }


        [Fact]
        public void ShouldInitializeContextWithDepartmentsSeed()
        {
            // Arrange
            var emptyDataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(emptyDataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            // Assert
            var allDepartments = this.Context.Set<Department>().ToList();
            allDepartments.Should().HaveCount(2);
        }

        [Fact(Skip = "Not implemented yet")]
        public void ShouldInitializeContextTwiceWithDepartmentsSeed()
        {
            // Arrange
            var emptyDataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(emptyDataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);
            var allDepartments1 = this.Context.Set<Department>().ToList();
            allDepartments1.Should().HaveCount(2);
            this.InitializeDatabase(databaseInitializer);

            // Assert
            var allDepartments2 = this.Context.Set<Department>().ToList();
            allDepartments2.Should().HaveCount(2);
        }
    }
}
