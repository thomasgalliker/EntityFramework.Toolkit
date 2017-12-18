using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using EntityFramework.Toolkit.Exceptions;
using EntityFramework.Toolkit.Extensions;
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

using static EntityFramework.Toolkit.Tests.Stubs.Testdata.Employees;

namespace EntityFramework.Toolkit.Tests.Repository
{
    /// <summary>
    ///     Repository tests using <see cref="EmployeeContextTestDbConnection" /> as database connection.
    /// </summary>
    public class GenericRepositoryTests : ContextTestBase<EmployeeContext, EmployeeContextTestDbConnection>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public GenericRepositoryTests(ITestOutputHelper testOutputHelper)
            : base(databaseInitializer: new CreateDatabaseIfNotExists<EmployeeContext>(),
                  log: testOutputHelper.WriteLine)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldAddEmployee()
        {
            // Arrange
            var employee = CreateEmployee1();
            employee.Department = Testdata.Departments.CreateDepartmentHumanResources();
            employee.Country = Testdata.Countries.CreateCountrySwitzerland();

            ChangeSet committedChangeSet;

            // Act
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.Add(employee);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 3, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var returnedEmployee = employeeRepository.Get().SingleOrDefault(e => e.FirstName == employee.FirstName);

                returnedEmployee.ShouldBeEquivalentTo(CreateEmployee1());
                returnedEmployee.CreatedDate.Should().BeAfter(DateTime.MinValue);
                returnedEmployee.UpdatedDate.Should().BeNull();
            }
        }

        [Fact]
        public void ShouldAddRangeOfEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateEmployee1(),
                CreateEmployee2(),
                CreateEmployee3()
            };
            ChangeSet committedChangeSet;

            // Act
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 3, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(3);
            }
        }

        [Fact]
        public void ShouldRemoveEmployee_Attached()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                removedEmployee = employeeRepository.Remove(employees.ElementAt(0));
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            removedEmployee.Should().NotBeNull();
            removedEmployee.ShouldBeEquivalentTo(CreateEmployee1());

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveEmployee_Detached()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var employeeToRemove = CreateEmployee1();
                employeeToRemove.Id = employees.ElementAt(0).Id;
                employeeToRemove.RowVersion = employees.ElementAt(0).RowVersion;

                removedEmployee = employeeRepository.Remove(employeeToRemove);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            removedEmployee.Should().NotBeNull();
            removedEmployee.ShouldBeEquivalentTo(CreateEmployee1());

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveAllEmployees()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2(), CreateEmployee3() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            IEnumerable<Employee> removedEmployees;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                removedEmployees = employeeRepository.RemoveAll().ToList();
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 3);

            removedEmployees.Should().HaveCount(3);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(0);
            }
        }

        [Fact]
        public void ShouldRemoveAllEmployeesWithCondition()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2(), CreateEmployee3() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.RemoveAll(e => e.FirstName == "Thomas");
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);
                allEmployees.Single(e => e.FirstName == CreateEmployee2().FirstName).ShouldBeEquivalentTo(CreateEmployee2());
                allEmployees.Single(e => e.FirstName == CreateEmployee3().FirstName).ShouldBeEquivalentTo(CreateEmployee3());
            }
        }

        [Fact]
        public void ShouldRemoveRangeEmployees()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2(), CreateEmployee3() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
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

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(CreateEmployee2());
            }
        }

        [Fact]
        public void ShouldRemoveEmployeeById()
        {
            // Arrange
            var employees = new List<Employee> { CreateEmployee1(), CreateEmployee2(), CreateEmployee3() };

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(employees);
                employeeRepository.Save();
            }

            // Act
            Employee removedEmployee;
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                removedEmployee = employeeRepository.RemoveById(employees[0].Id);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 0, expectedNumberOfDeleted: 1);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);
                removedEmployee.ShouldBeEquivalentTo(CreateEmployee1());
            }
        }

        [Fact]
        public void ShouldAddOrUpdateExistingEmployee_UpdateIfExists()
        {
            // Arrange
            var originalEmployee = CreateEmployee1();
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.Add(originalEmployee);
                employeeRepository.Save();
            }

            var updateEmployee = CreateEmployee1();
            updateEmployee.Id = originalEmployee.Id;
            updateEmployee.RowVersion = originalEmployee.RowVersion;
            updateEmployee.FirstName = "Updated " + updateEmployee.FirstName;

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                updateEmployee = employeeRepository.AddOrUpdate(updateEmployee);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 1, expectedNumberOfDeleted: 0);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
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
            var employee1Update = CreateEmployee1();
            employee1Update.FirstName = "Added " + employee1Update.FirstName;
            employee1Update.Country = Testdata.Countries.CreateCountrySwitzerland();

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employee1Update = employeeRepository.AddOrUpdate(employee1Update);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 2, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).Id.Should().Be(employee1Update.Id);
                allEmployees.ElementAt(0).FirstName.Should().Contain("Added");
            }
        }

        [Fact]
        public void ShouldUpdatePropertyOfExistingEmployee()
        {
            // Arrange
            var expectedFirstName = "Updated FirstName";
            var expectedLastName = "Updated LastName";
            var expectedEmployementDate = new DateTime(2000, 1, 1);

            var departmentHr = Testdata.Departments.CreateDepartmentHumanResources();
            var countryCH = Testdata.Countries.CreateCountrySwitzerland();

            Employee employee1;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employee1 = employeeRepository.Add(CreateEmployee1());
                employee1.Department = departmentHr;
                employee1.Country = countryCH;

                employeeRepository.Save();
            }

            employee1.FirstName = expectedFirstName;
            employee1.LastName = expectedLastName;

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.UpdateProperty(employee1, e => e.EmployementDate, expectedEmployementDate);
                employeeRepository.UpdateProperties(employee1, e => e.FirstName, e => e.LastName);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 1, expectedNumberOfDeleted: 0);
            var changedProperties = committedChangeSet.Changes.Single().ChangedProperties.ToList();
            changedProperties.Should().ContainSingle(p => p.PropertyName == "FirstName" && (string)p.CurrentValue == expectedFirstName);
            changedProperties.Should().ContainSingle(p => p.PropertyName == "LastName" && (string)p.CurrentValue == expectedLastName);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);

                var expectedEmployeeUpdate = CreateEmployee1();
                expectedEmployeeUpdate.FirstName = expectedFirstName;
                expectedEmployeeUpdate.LastName = expectedLastName;

                allEmployees.Single(e => e.Id == employee1.Id).ShouldBeEquivalentTo(expectedEmployeeUpdate);
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
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employee1 = employeeRepository.Add(CreateEmployee1());
                employee1.Department = departmentHr;
                employee1.Country = countryCH;

                employee2 = employeeRepository.Add(CreateEmployee2());
                employee2.Department = departmentHr;
                employee2.Country = countryCH;

                employeeRepository.Save();
            }

            var employeeUpdate = CreateEmployee1();
            employeeUpdate.Id = employee1.Id;
            employeeUpdate.RowVersion = employee1.RowVersion;
            employeeUpdate.DepartmentId = departmentHr.Id;
            employeeUpdate.CountryId = countryCH.Id;
            employeeUpdate.FirstName = "Updated " + employeeUpdate.FirstName;

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.Update(employeeUpdate);
                committedChangeSet = employeeRepository.Save();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 0, expectedNumberOfModified: 1, expectedNumberOfDeleted: 0);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(2);

                var expectedEmployeeUpdate = CreateEmployee1();
                expectedEmployeeUpdate.FirstName = "Updated " + expectedEmployeeUpdate.FirstName;

                var returnedEmployee1 = allEmployees.Single(e => e.Id == employee1.Id);
                returnedEmployee1.ShouldBeEquivalentTo(expectedEmployeeUpdate);
                returnedEmployee1.UpdatedDate.Should().BeAfter(DateTime.MinValue);

                var returnedEmployee2 = allEmployees.Single(e => e.Id == employee2.Id);
                returnedEmployee2.ShouldBeEquivalentTo(CreateEmployee2());
                returnedEmployee2.UpdatedDate.Should().BeNull();
            }
        }

        [Fact]
        public void ShouldUpdateExistingEmployees()
        {
            // Arrange
            var departmentHr = Testdata.Departments.CreateDepartmentHumanResources();

            var originalEmployees = new List<Employee>();
            var originalEmployee1 = CreateEmployee1();
            originalEmployee1.Department = departmentHr;

            var originalEmployee2 = CreateEmployee2();
            originalEmployee2.Department = departmentHr;

            var originalEmployee3 = CreateEmployee3();
            originalEmployee3.Department = departmentHr;

            originalEmployees.Add(originalEmployee1);
            originalEmployees.Add(originalEmployee2);
            originalEmployees.Add(originalEmployee3);

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.AddRange(originalEmployees);
                employeeRepository.Save();
            }

            int n = 5000;
            var timespanOffset = new TimeSpan(0, 0, 0, 1);
            var stopwatch = new Stopwatch();

            var updatedEmployees = new List<Employee>();
            var updatedEmployee1 = CreateEmployee1();
            updatedEmployee1.Id = originalEmployee1.Id;
            updatedEmployee1.RowVersion = originalEmployee1.RowVersion;

            var updatedEmployee2 = CreateEmployee2();
            updatedEmployee2.Id = originalEmployee2.Id;
            updatedEmployee2.RowVersion = originalEmployee2.RowVersion;

            var updatedEmployee3 = CreateEmployee3();
            updatedEmployee3.Id = originalEmployee3.Id;
            updatedEmployee3.RowVersion = originalEmployee3.RowVersion;

            updatedEmployees.Add(originalEmployee1);
            updatedEmployees.Add(originalEmployee2);
            updatedEmployees.Add(originalEmployee3);

            // Act
            ChangeSet committedChangeSet;
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
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

            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(3);
                allEmployees.ElementAt(0).Birthdate.Should().Be(new DateTime(1986, 07, 11, 01, 23, 20));
                allEmployees.ElementAt(1).Birthdate.Should().Be(new DateTime(1990, 01, 01, 01, 23, 20));
                allEmployees.ElementAt(2).Birthdate.Should().Be(new DateTime(2000, 12, 31, 01, 23, 20));

                stopwatch.ElapsedMilliseconds.Should().BeLessThan(2500);
            }
        }

        [Fact]
        public void ShouldThrowUpdateConcurrencyExceptionWhenTryingToUpdateNonExistingEntity()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.CreateContext());
            var employee1 = CreateEmployee1();
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