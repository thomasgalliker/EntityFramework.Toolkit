using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using EntityFramework.Toolkit.EF6.Extensions;
using EntityFramework.Toolkit.EF6.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class DbContextExtensionsTests : ContextTestBase<EmployeeContext>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public DbContextExtensionsTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: new DropCreateDatabaseAlways<EmployeeContext>(),
                   log: testOutputHelper.WriteLine)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldGetPrimaryKeyOfBaseEntity()
        {
            // Act
            var primaryKeyProperty = this.CreateContext().GetPrimaryKeyFor<Person>();

            // Assert
            primaryKeyProperty.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().BeReadable();
            primaryKeyProperty.PropertyInfo.Should().BeWritable();
            primaryKeyProperty.PropertyInfo.Name.Should().Be("Id");
            primaryKeyProperty.Value.Should().BeNull();
        }

        [Fact]
        public void ShouldGetPrimaryKeyOfDerivedEntity()
        {
            // Act
            var primaryKeyProperty = this.CreateContext().GetPrimaryKeyFor<Employee>();

            // Assert
            primaryKeyProperty.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().BeReadable();
            primaryKeyProperty.PropertyInfo.Should().BeWritable();
            primaryKeyProperty.PropertyInfo.Name.Should().Be("Id");
            primaryKeyProperty.Value.Should().BeNull();
        }

        [Fact]
        public void ShouldGetPrimaryKeyForEntityOfBaseEntity()
        {
            // Act
            int expectedPrimaryKeyValue = 99;
            var primaryKeyProperty = this.CreateContext().GetPrimaryKeyForEntity(new Person { Id = expectedPrimaryKeyValue });

            // Assert
            primaryKeyProperty.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Name.Should().Be("Id");
            primaryKeyProperty.Value.Should().Be(expectedPrimaryKeyValue);
        }

        [Fact]
        public void ShouldGetPrimaryKeyForEntityOfDerivedEntity()
        {
            // Act
            int expectedPrimaryKeyValue = 99;
            var primaryKeyProperty = this.CreateContext().GetPrimaryKeyForEntity(new Employee { Id = expectedPrimaryKeyValue });

            // Assert
            primaryKeyProperty.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Name.Should().Be("Id");
            primaryKeyProperty.Value.Should().Be(expectedPrimaryKeyValue);
        }

        [Fact]
        public void ShouldMergeEntities_CreateWithTransientEntity()
        {
            // Arrange
            using (var employeeContext = this.CreateContext())
            {
                var employee1 = Testdata.Employees.CreateEmployee1();
                employeeContext.Set<Employee>().Add(employee1);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext = this.CreateContext())
            {
                var employee2 = Testdata.Employees.CreateEmployee2();
                employee2.Id = 3;
                employee2.FirstName += " Created";
                var mergedEmployee = employeeContext.Merge(employee2);
                employeeContext.Entry(mergedEmployee).State.Should().Be(EntityState.Added);
                employeeContext.SaveChanges();
            }

            // Assert
            using (var employeeContext = this.CreateContext())
            {
                var all = employeeContext.Set<Employee>().ToList();
                all.Should().HaveCount(2);
            }
        }

        [Fact]
        public void ShouldMergeEntities_WithUpdate()
        {
            // Arrange
            Employee employee1;
            using (var employeeContext = this.CreateContext())
            {
                employee1 = Testdata.Employees.CreateEmployee1();
                employeeContext.Set<Employee>().Add(employee1);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext = this.CreateContext())
            {
                var employee1Update = Testdata.Employees.CreateEmployee1();
                employee1Update.Id = employee1.Id;
                employee1Update.RowVersion = employee1.RowVersion;
                employee1Update.FirstName += " Updated";
                employeeContext.Merge(employee1Update);
                employeeContext.SaveChanges();
            }

            // Assert
            using (var employeeContext = this.CreateContext())
            {
                var all = employeeContext.Set<Employee>().ToList();
                all.Should().HaveCount(1);
                all.ElementAt(0).FirstName.Should().Contain("Updated");
            }
        }

        [Fact]
        public void ShouldGetNavigationPropertiesFromBaseClass()
        {
            // Arrange
            List<PropertyInfo> navigationProperties;

            // Act
            using (var employeeContext = this.CreateContext())
            {
                navigationProperties = employeeContext.GetNavigationProperties<Person>();
            }

            // Assert
            navigationProperties.Should().HaveCount(1);
            navigationProperties.Should().ContainSingle(p => p.Name == "Country");
        }

        [Fact]
        public void ShouldGetNavigationPropertiesFromInheritedClass()
        {
            // Arrange
            List<PropertyInfo> navigationProperties;

            // Act
            using (var employeeContext = this.CreateContext())
            {
                navigationProperties = employeeContext.GetNavigationProperties(typeof(Employee));
            }

            // Assert
            navigationProperties.Should().HaveCount(2);
            navigationProperties.Should().ContainSingle(p => p.Name == "Country");
            navigationProperties.Should().ContainSingle(p => p.Name == "Department");
        }

        [Fact]
        public void ShouldGetGetTableRowCounts()
        {
            // Arrange
            List<TableRowCounts> tableRowCounts;

            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Set<Employee>().Add(Testdata.Employees.CreateEmployee1());
                employeeContext.Set<Employee>().Add(Testdata.Employees.CreateEmployee2());
                employeeContext.Set<Employee>().Add(Testdata.Employees.CreateEmployee3());
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext = this.CreateContext())
            {
                tableRowCounts = employeeContext.GetTableRowCounts();
            }

            // Assert
            tableRowCounts.Should().HaveCount(9);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[__MigrationHistory]" && r.TableRowCount == 1);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[ApplicationSettings]" && r.TableRowCount == 0);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Countries]" && r.TableRowCount == 0);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Departments]" && r.TableRowCount == 0);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Employee]" && r.TableRowCount == 3);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[EmployeeAudit]" && r.TableRowCount == 0);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Person]" && r.TableRowCount == 3);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Room]" && r.TableRowCount == 0);
            tableRowCounts.Should().ContainSingle(r => r.TableName == "[dbo].[Student]" && r.TableRowCount == 0);
        }
    }
}
