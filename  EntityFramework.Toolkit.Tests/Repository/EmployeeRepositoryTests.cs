using System.Collections.Generic;
using System.Data.Extensions.Extensions;
using System.Data.Extensions.Testing;
using System.Linq;

using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Model;
using ToolkitSample.DataAccess.Repository;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Repository
{
    public class EmployeeRepositoryTests : ContextTestBase<EmployeeContext>
    {
        public EmployeeRepositoryTests()
            : base(dbConnection: new EmployeeContextTestDbConnection())
        {
        }

        [Fact]
        public void ShouldReturnEmptyGetAll()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);

            // Act
            var allEmployees = employeeRepository.GetAll().ToList();

            // Assert
            allEmployees.Should().HaveCount(0);
        }

        [Fact]
        public void ShouldAddEmployee()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employee = CreateEntity.Employee1;

            // Act
            employeeRepository.Add(employee);
            var numberOfChangesCommitted = employeeRepository.Save();

            // Assert
            numberOfChangesCommitted.Should().BeGreaterThan(0);

            var getEmployee = employeeRepository.Get()
                .Include(d => d.Department)
                .Single(e => e.FirstName == employee.FirstName);

            getEmployee.ShouldBeEquivalentTo(CreateEntity.Employee1, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldAddRangeOfEmployees()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };

            // Act
            employeeRepository.AddRange(employees);
            var numberOfChangesCommitted = employeeRepository.Save();

            // Assert
            numberOfChangesCommitted.Should().BeGreaterThan(0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(3);
        }

        [Fact]
        public void ShouldRemoveEmployee()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2 };
            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            // Act
            employeeRepository.Remove(employees.First());
            var numberOfRemoves = +employeeRepository.Save();

            // Assert
            numberOfRemoves.Should().BeGreaterThan(0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEntity.Employee2, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldRemoveAllEmployees()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };
            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            // Act
            employeeRepository.RemoveAll();
            var numberOfRemoves = +employeeRepository.Save();

            // Assert
            numberOfRemoves.Should().BeGreaterThan(0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(0);
        }

        [Fact]
        public void ShouldRemoveAllEmployeesWithCondition()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };
            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            // Act
            employeeRepository.RemoveAll(e => e.FirstName == "Thomas");
            var numberOfRemoves = +employeeRepository.Save();

            // Assert
            numberOfRemoves.Should().BeGreaterThan(0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(2);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEntity.Employee2, options => options.IncludingAllDeclaredProperties());
            allEmployees.ElementAt(1).ShouldBeEquivalentTo(CreateEntity.Employee3, options => options.IncludingAllDeclaredProperties());
        }


        [Fact(Skip = "work in progress")]
        public void ShouldRemoveRangeEmployees()
        {
        }
    }
}