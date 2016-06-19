using System;
using System.Data.Entity.Infrastructure;
using System.Data.Extensions.Concurrency;
using System.Data.Extensions.Testing;
using System.Linq;

using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class DbContextBaseTests : ContextTestBase<EmployeeContext>
    {
        public DbContextBaseTests()
            : base(dbConnection: new EmployeeContextTestDbConnection())
        {
        }

        [Fact]
        public void ShouldRethrowConcurrencyUpdateExceptionAsDefault()
        {
            // Arrange
            var initialEmployee = CreateEntity.Employee1;

            string firstNameChange1 = initialEmployee.FirstName + " from employeeContext1";
            string firstNameChange2 = initialEmployee.FirstName + " from employeeContext2";

            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Set<Employee>().Add(initialEmployee);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext1 = this.CreateContext())
            {
                // Get an employee (which has a Version Timestamp) in one context and modify
                var employeeFromContext1 = employeeContext1.Set<Employee>().First(p => p.Id == 1);
                employeeFromContext1.FirstName = firstNameChange1;

                // Modify and Save the same employee in another context to simulate concurrent access
                using (var employeeContext2 = this.CreateContext())
                {
                    var employeeFromContext2 = employeeContext2.Set<Employee>().First(p => p.Id == 1);
                    employeeFromContext2.FirstName = firstNameChange2;
                    employeeContext2.SaveChanges();
                }

                // SaveChanges of the first context results in a DbUpdateConcurrencyException
                Action action = () => employeeContext1.SaveChanges();

                // Assert
                action.ShouldThrow<DbUpdateConcurrencyException>();
            }
        }

        [Fact]
        public void ShouldResolveConcurrencyExceptionWithDatabaseWinsStrategy()
        {
            // Arrange
            IConcurrencyResolveStrategy concurrencyResolveStrategy = new DatabaseWinsConcurrencyResolveStrategy();
            var initialEmployee = CreateEntity.Employee1;

            string firstNameChange1 = initialEmployee.FirstName + " from employeeContext1";
            string firstNameChange2 = initialEmployee.FirstName + " from employeeContext2";

            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Set<Employee>().Add(initialEmployee);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext1 = this.CreateContext())
            {
                employeeContext1.ConcurrencyResolveStrategy = concurrencyResolveStrategy;

                // Get an employee (which has a Version Timestamp) in one context and modify
                var employeeFromContext1 = employeeContext1.Set<Employee>().First(p => p.Id == 1);
                employeeFromContext1.FirstName = firstNameChange1;

                // Modify and Save the same employee in another context to simulate concurrent access
                using (var employeeContext2 = this.CreateContext())
                {
                    var employeeFromContext2 = employeeContext2.Set<Employee>().First(p => p.Id == 1);
                    employeeFromContext2.FirstName = firstNameChange2;
                    employeeContext2.SaveChanges();
                }

                // SaveChanges of the first context results in a DbUpdateConcurrencyException
                employeeContext1.SaveChanges();
            }

            // Assert
            using (var employeeContext = this.CreateContext())
            {
                var returnedEmployee = employeeContext.Set<Employee>().First(p => p.Id == 1);
                returnedEmployee.FirstName.Should().Be(firstNameChange2);
            }
        }

        [Fact]
        public void ShouldResolveConcurrencyExceptionWithClientWinsStrategy()
        {
            // Arrange
            IConcurrencyResolveStrategy concurrencyResolveStrategy = new ClientWinsConcurrencyResolveStrategy();
            var initialEmployee = CreateEntity.Employee1;

            string firstNameChange1 = initialEmployee.FirstName + " from employeeContext1";
            string firstNameChange2 = initialEmployee.FirstName + " from employeeContext2";

            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Set<Employee>().Add(initialEmployee);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext1 = this.CreateContext())
            {
                employeeContext1.ConcurrencyResolveStrategy = concurrencyResolveStrategy;

                // Get an employee (which has a Version Timestamp) in one context and modify
                var employeeFromContext1 = employeeContext1.Set<Employee>().First(p => p.Id == 1);
                employeeFromContext1.FirstName = firstNameChange1;

                // Modify and Save the same employee in another context to simulate concurrent access
                using (var employeeContext2 = this.CreateContext())
                {
                    var employeeFromContext2 = employeeContext2.Set<Employee>().First(p => p.Id == 1);
                    employeeFromContext2.FirstName = firstNameChange2;
                    employeeContext2.SaveChanges();
                }

                // SaveChanges of the first context results in a DbUpdateConcurrencyException
                employeeContext1.SaveChanges();
            }

            // Assert
            using (var employeeContext = this.CreateContext())
            {
                var returnedEmployee = employeeContext.Set<Employee>().First(p => p.Id == 1);
                returnedEmployee.FirstName.Should().Be(firstNameChange1);
            }
        }
    }
}