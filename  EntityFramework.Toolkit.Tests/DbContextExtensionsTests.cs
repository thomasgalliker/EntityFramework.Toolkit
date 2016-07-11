using System.Data.Entity;
using System.Linq;

using EntityFramework.Toolkit.Extensions;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class DbContextExtensionsTests : ContextTestBase<EmployeeContext>
    {
        public DbContextExtensionsTests()
            : base(dbConnection: new EmployeeContextTestDbConnection())
        {
        }

        [Fact]
        public void ShouldGetPrimaryKeyFor()
        {
            // Act
            var primaryKeyProperty = this.Context.GetPrimaryKeyFor<Employee>();

            // Assert
            primaryKeyProperty.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().NotBeNull();
            primaryKeyProperty.PropertyInfo.Should().BeReadable();
            primaryKeyProperty.PropertyInfo.Should().BeWritable();
            primaryKeyProperty.PropertyInfo.Name.Should().Be("Id");
        }

        [Fact]
        public void ShouldMergeEntities_CreateWithTransientEntity()
        {
            // Arrange
            using (var employeeContext = this.CreateContext())
            {
                var employee1 = CreateEntity.Employee1;
                employeeContext.Set<Employee>().Add(employee1);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext = this.CreateContext())
            {
                var employee2 = CreateEntity.Employee2;
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
            using (var employeeContext = this.CreateContext())
            {
                var employee1 = CreateEntity.Employee1;
                employeeContext.Set<Employee>().Add(employee1);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext = this.CreateContext())
            {
                var employee1 = CreateEntity.Employee1;
                employee1.FirstName += " Updated";
                employeeContext.Merge(employee1);
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
    }
}
