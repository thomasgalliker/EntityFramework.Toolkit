using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests.Extensions
{
    /// <summary>
    ///     Repository tests using <see cref="EmployeeContextTestDbConnection" /> as database connection.
    /// </summary>
    public class DbUpdateExceptionFormatterTests : ContextTestBase<EmployeeContext>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public DbUpdateExceptionFormatterTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: new CreateDatabaseIfNotExists<EmployeeContext>(),
                  log: testOutputHelper.WriteLine)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldThrowDbUpdateExceptionWithFormattedExceptionMessage()
        {
            // Arrange
            var employee = Testdata.Employees.CreateEmployee1();
            employee.DepartmentId = 99;
            employee.CountryId = "XX";

            // Act
            using (IGenericRepository<Employee> employeeRepository = new GenericRepository<Employee>(this.CreateContext()))
            {
                employeeRepository.Add(employee);
                Action action = () => employeeRepository.Save();

                // Assert
                action.ShouldThrow<DbUpdateException>()
                    .Which.Message.Should()
                    .Contain("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_dbo.Person_dbo.Countries_CountryId\".")
                    .And.Contain("(X) CountryId: Type: String, Value: \"XX\"");
            }
        }
    }
}