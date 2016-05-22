using System.Data.Extensions.Testing;
using System.Linq;

using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
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
            numberOfChangesCommitted.Should().Be(1);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee, options => options.IncludingAllDeclaredProperties());

        }

        [Fact]
        public void ShouldDeleteEmployee()
        {
            // Arrange
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.Context);
            var employee1 = CreateEntity.Employee1;
            var employee2 = CreateEntity.Employee2;

            employeeRepository.Add(employee1);
            employeeRepository.Add(employee2);
            var numberOfAdds = +employeeRepository.Save();

            // Act
            employeeRepository.Delete(employee2);
            var numberOfDeletes = +employeeRepository.Save();

            // Assert
            numberOfAdds.Should().Be(2);
            numberOfDeletes.Should().Be(1);

            var allEmployees = employeeRepository.GetAll().ToList();
            allEmployees.Should().HaveCount(1);
            allEmployees.ElementAt(0).ShouldBeEquivalentTo(employee1, options => options.IncludingAllDeclaredProperties());
        }
    }
}