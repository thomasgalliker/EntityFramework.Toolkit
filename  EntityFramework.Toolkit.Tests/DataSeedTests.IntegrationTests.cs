using System.Linq;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Testing;

using FluentAssertions;

using ToolkitSample.DataAccess;
using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Migrations;
using ToolkitSample.DataAccess.Seed;
using ToolkitSample.Model;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class DataSeedTests_IntegrationTests : ContextTestBase<EmployeeContext>
    {
        public DataSeedTests_IntegrationTests()
            : base(dbConnection: new EmployeeContextTestDbConnection(), initializeDatabase: false, databaseInitializer: null)
        {
        }

        [Fact]
        public void ShouldInitializeContextWithEmptyDataSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

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
            var dataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            // Assert
            var allDepartments = this.Context.Set<Department>().ToList();
            allDepartments.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldInitializeContextWithApplicationSettingSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new ApplicationSettingDataSeed(),  };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            // Assert
            var applicationSetting = this.Context.Set<ApplicationSetting>().ToList();
            applicationSetting.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldInitializeContextTwiceWithDepartmentsSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            var allDepartments1 = this.Context.Set<Department>().ToList();
            allDepartments1.Should().HaveCount(2);

            this.InitializeDatabase(databaseInitializer);

            // Assert
            var allDepartments2 = this.Context.Set<Department>().ToList();
            allDepartments2.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldInitializeContextTwiceWithApplicationSettingSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new ApplicationSettingDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            this.InitializeDatabase(databaseInitializer);

            var applicationSettings1 = this.Context.Set<ApplicationSetting>().ToList();
            applicationSettings1.Should().HaveCount(1);

            this.InitializeDatabase(databaseInitializer);

            // Assert
            var applicationSettings2 = this.Context.Set<ApplicationSetting>().ToList();
            applicationSettings2.Should().HaveCount(1);
        }
    }
}