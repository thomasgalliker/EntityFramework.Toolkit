﻿using System.Data.Entity;
using System.Linq;

using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.DataAccess.Repository;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Repository
{
    public class EmployeeRepositoryTests
    {
        [Fact]
        public void ShouldReturnEmptyGetAll()
        {
            // Arrange
            var dbConnection = new EmployeeContextTestDbConnection();
            var dataInitializer = new DropCreateDatabaseAlways<EmployeeContext>();

            using (IEmployeeContext employeeContext = new EmployeeContext(dbConnection, dataInitializer))
            {
                IEmployeeRepository employeeRepository = new EmployeeRepository(employeeContext);

                // Act
                var allEmployees = employeeRepository.GetAll().ToList();

                 // Assert
                allEmployees.Should().HaveCount(0);
            }
        }

        [Fact]
        public void ShouldAddEmployee()
        {
            // Arrange
            var dbConnection = new EmployeeContextTestDbConnection();
            var dataInitializer = new DropCreateDatabaseAlways<EmployeeContext>();

            using (IEmployeeContext employeeContext = new EmployeeContext(dbConnection, dataInitializer))
            {
                IEmployeeRepository employeeRepository = new EmployeeRepository(employeeContext);
                var employee = CreateEntity.Employee1;

                // Act
                employeeRepository.Add(employee);
                var numberOfChangesCommitted = employeeContext.SaveChanges();

                // Assert
                numberOfChangesCommitted.Should().Be(1);

                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee, options => options.IncludingAllDeclaredProperties());
            }
        }

        [Fact]
        public void ShouldDeleteEmployee()
        {
            // Arrange
            var dbConnection = new EmployeeContextTestDbConnection();
            var dataInitializer = new DropCreateDatabaseAlways<EmployeeContext>();

            using (IEmployeeContext employeeContext = new EmployeeContext(dbConnection, dataInitializer))
            {
                IEmployeeRepository employeeRepository = new EmployeeRepository(employeeContext);
                var employee1 = CreateEntity.Employee1;
                var employee2 = CreateEntity.Employee2;

                employeeRepository.Add(employee1);
                employeeRepository.Add(employee2);
                var numberOfAdds = +employeeContext.SaveChanges();

                // Act
                employeeRepository.Delete(employee2);
                var numberOfDeletes =+ employeeContext.SaveChanges();

                // Assert
                numberOfAdds.Should().Be(2);
                numberOfDeletes.Should().Be(1);

                var allEmployees = employeeRepository.GetAll().ToList();
                allEmployees.Should().HaveCount(1);
                allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee1, options => options.IncludingAllDeclaredProperties());
            }
        }
    }
}