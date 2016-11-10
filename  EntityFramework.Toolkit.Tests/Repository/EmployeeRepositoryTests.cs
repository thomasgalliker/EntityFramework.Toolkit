using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using EntityFramework.Toolkit.Core.Extensions;
using EntityFramework.Toolkit.Exceptions;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Extensions;
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
    ///     Repository tests using <see cref="EmployeeContextTestDbConnection" /> as database connection.
    /// </summary>
    public class EmployeeRepositoryTests : ContextTestBase<EmployeeContext>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public EmployeeRepositoryTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  initializer: new CreateDatabaseIfNotExists<EmployeeContext>(),
                  log: testOutputHelper.WriteLine)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldAddEmployee()
        {
            // Arrange
            var employee = Testdata.Employees.CreateEmployee1();
            employee.Department = Testdata.Departments.CreateDepartmentHumanResources();
            employee.Country = Testdata.Countries.CreateCountrySwitzerland();

            ChangeSet committedChangeSet;

            // Act
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.Add(employee);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 3, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var returnedEmployee = employeeRepository.Get().SingleOrDefault(e => e.FirstName == employee.FirstName);

                returnedEmployee.ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee1());
            }
        }

        [Fact]
        public void ShouldAddRangeOfEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                Testdata.Employees.CreateEmployee1(),
                Testdata.Employees.CreateEmployee2(),
                Testdata.Employees.CreateEmployee3()
            };
            ChangeSet committedChangeSet;

            // Act
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 3, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(3);
            }
        }

        [Fact]
        public void ShouldRemoveEmployee_Attached()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                removedEmployee = employeeRepository.Remove(employees.ElementAt(0));
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            removedEmployee.Should().NotBeNull();
            removedEmployee.ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee1());

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveEmployee_Detached()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var employeeToRemove = Testdata.Employees.CreateEmployee1();
                employeeToRemove.Id = employees.ElementAt(0).Id;
                employeeToRemove.RowVersion = employees.ElementAt(0).RowVersion;

                removedEmployee = employeeRepository.Remove(employeeToRemove);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            removedEmployee.Should().NotBeNull();
            removedEmployee.ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee1());

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveAllEmployees()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            IEnumerable<Employee> removedEmployees;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                removedEmployees = employeeRepository.RemoveAll().ToList();
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 3);

            removedEmployees.Should().HaveCount(3);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(0);
            }
        }

        [Fact]
        public void ShouldRemoveAllEmployeesWithCondition()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.RemoveAll(e => e.FirstName == "Thomas");
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);
                allEmployees.Single(e => e.FirstName == Testdata.Employees.CreateEmployee2().FirstName).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee2());
                allEmployees.Single(e => e.FirstName == Testdata.Employees.CreateEmployee3().FirstName).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee3());
            }
        }

        [Fact]
        public void ShouldRemoveRangeEmployees()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var employeeToRemove1 = new Employee();
                employeeToRemove1.Id = employees.ElementAt(0).Id;
                employeeToRemove1.RowVersion = employees.ElementAt(0).RowVersion;

                var employeeToRemove3 = new Employee();
                employeeToRemove3.Id = employees.ElementAt(2).Id;
                employeeToRemove3.RowVersion = employees.ElementAt(2).RowVersion;

                employeeRepository.RemoveRange(new[] { employeeToRemove1, employeeToRemove3 });
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 2);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveEmployeeById()
        {
            // Arrange
            var employees = new List<Employee> { Testdata.Employees.CreateEmployee1(), Testdata.Employees.CreateEmployee2(), Testdata.Employees.CreateEmployee3() };

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                removedEmployee = employeeRepository.RemoveById(employees[0].Id);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);
                removedEmployee.ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee1());
            }
        }

        [Fact]
        public void ShouldAddOrUpdateExistingEmployee_UpdateIfExists()
        {
            // Arrange
            var originalEmployee = Testdata.Employees.CreateEmployee1();
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.Add(originalEmployee);
                employeeRepository.Save();
            }

            var updateEmployee = Testdata.Employees.CreateEmployee1();
            updateEmployee.Id = originalEmployee.Id;
            updateEmployee.RowVersion = originalEmployee.RowVersion;
            updateEmployee.FirstName = "Updated " + updateEmployee.FirstName;

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                updateEmployee = employeeRepository.AddOrUpdate(updateEmployee);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 1, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).FirstName.Should().Contain("Updated");
            }
        }

        [Fact]
        public void ShouldAddOrUpdateExistingEmployee_AddIfDoesNotExist()
        {
            // Arrange
            var employee1Update = Testdata.Employees.CreateEmployee1();
            employee1Update.FirstName = "Added " + employee1Update.FirstName;
            employee1Update.Country = Testdata.Countries.CreateCountrySwitzerland();

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employee1Update = employeeRepository.AddOrUpdate(employee1Update);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 2, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).Id.Should().Be(employee1Update.Id);
                allEmployees.ElementAt(0).FirstName.Should().Contain("Added");
            }
        }

        [Fact]
        public void ShouldUpdateExistingEmployee()
        {
            // Arrange
            var departmentHr = Testdata.Departments.CreateDepartmentHumanResources();
            var countryCH = Testdata.Countries.CreateCountrySwitzerland();

            Employee employee1;
            Employee employee2;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employee1 = employeeRepository.Add(Testdata.Employees.CreateEmployee1());
                employee1.Department = departmentHr;
                employee1.Country = countryCH;

                employee2 = employeeRepository.Add(Testdata.Employees.CreateEmployee2());
                employee2.Department = departmentHr;
                employee2.Country = countryCH;

                employeeRepository.Save();
            }

            var employeeUpdate = Testdata.Employees.CreateEmployee1();
            employeeUpdate.Id = employee1.Id;
            employeeUpdate.RowVersion = employee1.RowVersion;
            employeeUpdate.DepartmentId = departmentHr.Id;
            employeeUpdate.CountryId = countryCH.Id;
            employeeUpdate.FirstName = "Updated " + employeeUpdate.FirstName;

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.Update(employeeUpdate);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 1, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);

                var expectedEmployeeUpdate = Testdata.Employees.CreateEmployee1();
                expectedEmployeeUpdate.FirstName = "Updated " + expectedEmployeeUpdate.FirstName;

                allEmployees.Single(e => e.Id == employee1.Id).ShouldBeEquivalentTo(expectedEmployeeUpdate);
                allEmployees.Single(e => e.Id == employee2.Id).ShouldBeEquivalentTo(Testdata.Employees.CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldUpdateExistingEmployees()
        {
            // Arrange
            var departmentHr = Testdata.Departments.CreateDepartmentHumanResources();

            var originalEmployees = new List<Employee>();
            var originalEmployee1 = Testdata.Employees.CreateEmployee1();
            originalEmployee1.Department = departmentHr;

            var originalEmployee2 = Testdata.Employees.CreateEmployee2();
            originalEmployee2.Department = departmentHr;

            var originalEmployee3 = Testdata.Employees.CreateEmployee3();
            originalEmployee3.Department = departmentHr;

            originalEmployees.Add(originalEmployee1);
            originalEmployees.Add(originalEmployee2);
            originalEmployees.Add(originalEmployee3);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                employeeRepository.AddRange(originalEmployees);
                employeeRepository.Save();
            }

            int n = 5000;
            var timespanOffset = new TimeSpan(0, 0, 0, 1);
            var stopwatch = new Stopwatch();

            var updatedEmployees = new List<Employee>();
            var updatedEmployee1 = Testdata.Employees.CreateEmployee1();
            updatedEmployee1.Id = originalEmployee1.Id;
            updatedEmployee1.RowVersion = originalEmployee1.RowVersion;

            var updatedEmployee2 = Testdata.Employees.CreateEmployee2();
            updatedEmployee2.Id = originalEmployee2.Id;
            updatedEmployee2.RowVersion = originalEmployee2.RowVersion;

            var updatedEmployee3 = Testdata.Employees.CreateEmployee3();
            updatedEmployee3.Id = originalEmployee3.Id;
            updatedEmployee3.RowVersion = originalEmployee3.RowVersion;

            updatedEmployees.Add(originalEmployee1);
            updatedEmployees.Add(originalEmployee2);
            updatedEmployees.Add(originalEmployee3);

            // Act
            ChangeSet committedChangeSet;
            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                stopwatch.Start();
                for (int i = 0; i < n; i++)
                {
                    foreach (var updateEmployee in updatedEmployees)
                    {
                        updateEmployee.Birthdate += timespanOffset;
                        employeeRepository.Update(updateEmployee);
                    }
                }
                committedChangeSet = employeeRepository.Save();
                stopwatch.Stop();
            }

            this.testOutputHelper.WriteLine($"Elapsed={stopwatch.ElapsedMilliseconds}ms");

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 3, expectedNumberOfDeleted: 0);

            using (IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(3);
                allEmployees.ElementAt(0).Birthdate.Should().Be(new DateTime(1986, 07, 11, 01, 23, 20));
                allEmployees.ElementAt(1).Birthdate.Should().Be(new DateTime(1990, 01, 01, 01, 23, 20));
                allEmployees.ElementAt(2).Birthdate.Should().Be(new DateTime(2000, 12, 31, 01, 23, 20));

                stopwatch.ElapsedMilliseconds.Should().BeLessThan(1500);
            }
        }

        [Fact]
        public void ShouldThrowUpdateConcurrencyExceptionWhenTryingToUpdateNonExistingEntity()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext());
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
    }
}