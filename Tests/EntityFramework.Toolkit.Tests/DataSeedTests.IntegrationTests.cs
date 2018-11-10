using System.Collections.Generic;
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
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class DataSeedIntegrationTests : ContextTestBase<EmployeeContext>
    {
        public DataSeedIntegrationTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: null,
                  log: testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldInitializeContextWithEmptyDataSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            var context = this.CreateContext(databaseInitializer);

            // Assert
            var allDepartments = context.Set<Department>().ToList();
            allDepartments.Should().HaveCount(0);
        }

        [Fact]
        public void ShouldInitializeContextWithDepartmentsSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            var context = this.CreateContext(databaseInitializer);

            // Assert
            var allDepartments = context.Set<Department>().ToList();
            allDepartments.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldInitializeContextWithApplicationSettingSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new ApplicationSettingDataSeed(), };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            var context = this.CreateContext(databaseInitializer);

            // Assert
            var applicationSetting = context.Set<ApplicationSetting>().ToList();
            applicationSetting.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldInitializeContextTwiceWithDepartmentsSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new DepartmentDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            List<Department> allDepartmentsFirst;
            using (var context = this.CreateContext(databaseInitializer))
            {
                allDepartmentsFirst = context.Set<Department>().ToList();
            }

            List<Department> allDepartmentsSecond;
            using (var context = this.CreateContext(databaseInitializer))
            {
                allDepartmentsSecond = context.Set<Department>().ToList();
            }

            // Assert
            allDepartmentsFirst.Should().HaveCount(2);
            allDepartmentsSecond.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldInitializeContextTwiceWithApplicationSettingSeed()
        {
            // Arrange
            var dataSeed = new IDataSeed[] { new ApplicationSettingDataSeed() };
            var databaseInitializer = new EmployeeContextDatabaseInitializer(new EmployeeContextMigrationConfiguration(dataSeed));

            // Act
            List<ApplicationSetting> allDepartmentsFirst;
            using (var context = this.CreateContext(databaseInitializer))
            {
                allDepartmentsFirst = context.Set<ApplicationSetting>().ToList();
            }

            List<ApplicationSetting> allDepartmentsSecond;
            using (var context = this.CreateContext(databaseInitializer))
            {
                allDepartmentsSecond = context.Set<ApplicationSetting>().ToList();
            }

            // Assert
            allDepartmentsFirst.Should().HaveCount(1);
            allDepartmentsSecond.Should().HaveCount(1);
        }
    }
}