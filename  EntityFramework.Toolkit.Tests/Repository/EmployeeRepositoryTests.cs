using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Exceptions;
using EntityFramework.Toolkit.Extensions;
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
    /// <summary>
    /// Repository tests using <see cref="EmployeeContextTestDbConnection"/> as database connection.
    /// </summary>
    public class EmployeeRepositoryTests : ContextTestBase<EmployeeContext>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public EmployeeRepositoryTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: new EmployeeContextTestDbConnection())
        {
            this.testOutputHelper = testOutputHelper;
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
        public void ShouldGetAnyTrueIfEmployeeExists()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };

            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var expectedId = employees[0].Id;

            // Act
            var hasAny = employeeRepository.Any(expectedId);

            // Assert
            hasAny.Should().BeTrue();
        }

        [Fact]
        public void ShouldGetAnyFalseIfEmployeeDoesNotExist()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);

            // Act
            var hasAny = employeeRepository.Any(0);

            // Assert
            hasAny.Should().BeFalse();
        }

        [Fact]
        public void ShouldFindEmployeeById()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };

            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var expectedId = employees[0].Id;

            // Act
            var findByIdResult = employeeRepository.FindById(expectedId);

            // Assert
            findByIdResult.Should().NotBeNull();
            findByIdResult.Id.Should().Be(expectedId);
        }
        
        [Fact]
        public void ShouldFindByFirstName()
        {
            // Arrange
            string expectedFirstName = "Thomas";

            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };

            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            employeeRepository = new EmployeeRepository(this.CreateContext());

            // Act
            var findByResult = employeeRepository.FindBy(employee => employee.FirstName == expectedFirstName);

            // Assert
            findByResult.Should().HaveCount(1);
            findByResult.ElementAt(0).FirstName.Should().Be(expectedFirstName);
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
            var removedEmployee = employeeRepository.Remove(employees.First());
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 1);

            removedEmployee.Should().NotBeNull();
            removedEmployee.ShouldBeEquivalentTo(CreateEntity.Employee1, options => options.IncludingAllDeclaredProperties());

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
            employeeRepository.RemoveRange(new[] { CreateEntity.Employee1, CreateEntity.Employee3 });
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 2);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEntity.Employee2, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldRemoveEmployeeById()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { CreateEntity.Employee1, CreateEntity.Employee2, CreateEntity.Employee3 };
            employeeRepository.AddRange(employees);
            employeeRepository.Save();

            // Act
            var removedEmployee = employeeRepository.RemoveById(employees[0].Id);
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 0, numberOfDeleted: 1);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(2);
            removedEmployee.ShouldBeEquivalentTo(CreateEntity.Employee1, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldAddOrUpdateExistingEmployee_UpdateIfExists()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employee1 = employeeRepository.Add(CreateEntity.Employee1);
            employeeRepository.Save();

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var employee1Update = CreateEntity.Employee1;
            employee1Update.FirstName = "Updated " + employee1Update.FirstName;

            // Act
            employee1Update = employeeRepository.AddOrUpdate(employee1Update);
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 1, numberOfDeleted: 0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee1Update, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldAddOrUpdateExistingEmployee_AddIfDoesNotExist()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employee1Update = CreateEntity.Employee1;
            employee1Update.FirstName = "Updated " + employee1Update.FirstName;

            // Act
            employee1Update = employeeRepository.AddOrUpdate(employee1Update);
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 1, numberOfModified: 0, numberOfDeleted: 0);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee1Update, options => options.IncludingAllDeclaredProperties());
        }

        [Fact]
        public void ShouldUpdateExistingEmployee()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var departmentHr = Testdata.Departments.CreateDepartmentHumanResources();
            var employee1 = employeeRepository.Add(CreateEntity.Employee1);
            employee1.Department = departmentHr;
            var employee2 = employeeRepository.Add(CreateEntity.Employee2);
            employee2.Department = departmentHr;
            employeeRepository.Save();

            var employee1Update = CreateEntity.Employee1;
            employee1Update.FirstName = "Updated " + employee1Update.FirstName;

            // Act
            employeeRepository = new EmployeeRepository(this.CreateContext());
            employeeRepository.Update(employee1Update);
            var committedChangeSet = employeeRepository.Save();

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 1, numberOfDeleted: 0);

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(2);
            allEmployees.Single(e => e.Id == employee1.Id).ShouldBeEquivalentTo(employee1Update, options => options.IncludingAllDeclaredProperties().IgnoringCyclicReferences());
            allEmployees.Single(e => e.Id == employee2.Id).ShouldBeEquivalentTo(employee2, options => options.IncludingAllDeclaredProperties().IgnoringCyclicReferences());
        }

        [Fact]
        public void ShouldUpdateExistingEmployees()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };
            employees = employeeRepository.AddRange(employees).ToList();
            employeeRepository.Save();

            employeeRepository = new EmployeeRepository(this.CreateContext());
            int n = 5000;
            var timespanOffset = new TimeSpan(0, 0, 0, 1);

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < n; i++)
            {
                foreach (var updateEmployee in employees)
                {
                    updateEmployee.Birthdate += timespanOffset;
                    employeeRepository.Update(updateEmployee);
                }
            }
            var committedChangeSet = employeeRepository.Save();
            stopwatch.Stop();
            this.testOutputHelper.WriteLine($"Elapsed={stopwatch.ElapsedMilliseconds}ms");

            // Assert
            AssertChangeSet(committedChangeSet, numberOfAdded: 0, numberOfModified: 3, numberOfDeleted: 0);

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(3);
            allEmployees.ElementAt(0).Birthdate.Should().Be(new DateTime(1986, 07, 11, 01, 23, 20));
            allEmployees.ElementAt(1).Birthdate.Should().Be(new DateTime(1990, 01, 01, 01, 23, 20));
            allEmployees.ElementAt(2).Birthdate.Should().Be(new DateTime(2000, 12, 31, 01, 23, 20));

            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        }

        [Fact]
        public void ShouldThrowUpdateConcurrencyExceptionWhenTryingToUpdateNonExistingEntity()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employee1 = Testdata.Employees.CreateEmployee1();
            employee1.Id = 99;

            // Act
            employeeRepository.Update(employee1);
            Action action = () => employeeRepository.Save();

            // Assert
            action.ShouldThrow<UpdateConcurrencyException>();

            employeeRepository = new EmployeeRepository(this.CreateContext());
            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(0);
        }

        private static void AssertChangeSet(ChangeSet changeSet, int numberOfAdded, int numberOfModified, int numberOfDeleted)
        {
            changeSet.Changes.Where(c => c.State == ChangeState.Added).Should().HaveCount(numberOfAdded, $"Number of added should be {numberOfAdded}.");
            changeSet.Changes.Where(c => c.State == ChangeState.Modified).Should().HaveCount(numberOfModified, $"Number of modified should be {numberOfModified}.");
            changeSet.Changes.Where(c => c.State == ChangeState.Deleted).Should().HaveCount(numberOfDeleted, $"Number of deleted should be {numberOfDeleted}.");
        }
    }
}