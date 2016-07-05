using System.Collections.Generic;
using System.Linq;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.DataAccess.Repository;
using ToolkitSample.Model;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Repository
{
    /// <summary>
    /// Repository tests using <see cref="EmployeeContextTestDbConnection"/> as database connection.
    /// </summary>
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
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 1, numberOfModified: 0, numberOfDeleted: 0);

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
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 3, numberOfModified: 0, numberOfDeleted: 0);

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
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 1);

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
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 3);

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
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 1);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(2);
            allEmployees.Single(e => e.FirstName == CreateEntity.Employee2.FirstName).ShouldBeEquivalentTo(CreateEntity.Employee2, options => options.IncludingAllDeclaredProperties());
            allEmployees.Single(e => e.FirstName == CreateEntity.Employee3.FirstName).ShouldBeEquivalentTo(CreateEntity.Employee3, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldRemoveRangeEmployees()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };
            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            // Act
            employeeRepository.RemoveRange(new [] { CreateEntity.Employee1, CreateEntity.Employee3 });
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 2);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEntity.Employee2, options => options.IncludingAllDeclaredProperties());
        }

        private static void AssertChangeSet(ChangeSet changeSet, int numberOfAdded, int numberOfModified, int numberOfDeleted)
        {
            changeSet.Changes.Where(c => c.State == ChangeState.Added).Should().HaveCount(numberOfAdded);
            changeSet.Changes.Where(c => c.State == ChangeState.Modified).Should().HaveCount(numberOfModified);
            changeSet.Changes.Where(c => c.State == ChangeState.Deleted).Should().HaveCount(numberOfDeleted);
        }
    }
}