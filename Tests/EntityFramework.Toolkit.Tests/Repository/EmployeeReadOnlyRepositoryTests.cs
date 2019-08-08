using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Core.Extensions;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;
using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.DataAccess.Repository;
using ToolkitSample.Model;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests.Repository
{
    public class EmployeeReadOnlyRepositoryTests : ContextTestBase<EmployeeContext>
    {
        public EmployeeReadOnlyRepositoryTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: new CreateDatabaseIfNotExists<EmployeeContext>(),
                  log: testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldReturnEmptyGetAll()
        {
            // Arrange
            IReadOnlyRepository<Employee> employeeRepository = new EmployeeReadOnlyRepository(this.CreateContext());

            // Act
            var allEmployees = employeeRepository.GetAll().ToList();

            // Assert
            allEmployees.Should().HaveCount(0);
        }

        [Fact]
        public void ShouldGetAnyTrueIfEmployeeExists()
        {
            // Arrange
            var employees = new List<Employee>
            {
                Testdata.Employees.CreateEmployee1(),
                Testdata.Employees.CreateEmployee2(),
                Testdata.Employees.CreateEmployee3()
            };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            var expectedId = employees[0].Id;

            // Act
            bool hasAny;
            using (IEmployeeReadOnlyRepository employeeRepository = new EmployeeReadOnlyRepository(this.CreateContext()))
            {
                hasAny = employeeRepository.Any(expectedId);
            }

            // Assert
            hasAny.Should().BeTrue();
        }

        [Fact]
        public void ShouldGetAnyTrueIfEmployeeExists_WithExpression()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            var expectedId = employees[0].Id;

            // Act
            bool hasAny;
            using (IEmployeeReadOnlyRepository employeeRepository = new EmployeeReadOnlyRepository(this.CreateContext()))
            {
                hasAny = employeeRepository.Any(e => e.Id == expectedId);
            }

            // Assert
            hasAny.Should().BeTrue();
        }

        [Fact]
        public void ShouldGetAnyFalseIfEmployeeDoesNotExist()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext());

            // Act
            var hasAny = employeeRepository.Any(0);

            // Assert
            hasAny.Should().BeFalse();
        }

        [Fact]
        public void ShouldFindEmployeeById()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            var expectedId = employees[0].Id;

            // Act
            Employee foundEmployee;
            using (IEmployeeReadOnlyRepository employeeRepository = new EmployeeReadOnlyRepository(this.CreateContext()))
            {
                foundEmployee = employeeRepository.FindById(expectedId);
            }

            // Assert
            foundEmployee.Should().NotBeNull();
            foundEmployee.Id.Should().Be(expectedId);
        }

        [Fact]
        public void ShouldFindByFirstName()
        {
            // Arrange
            string expectedFirstName = "Thomas";

            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            IEnumerable<Employee> foundEmployees;
            using (IEmployeeReadOnlyRepository employeeRepository = new EmployeeReadOnlyRepository(this.CreateContext()))
            {
                foundEmployees = employeeRepository.FindBy(employee => employee.FirstName == expectedFirstName).ToList();
            }

            // Assert
            foundEmployees.Should().HaveCount(1);
            foundEmployees.ElementAt(0).FirstName.Should().Be(expectedFirstName);
        }
    }
}