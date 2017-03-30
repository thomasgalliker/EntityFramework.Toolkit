using System.Linq;



using FluentAssertions;

using ToolkitSample.DataAccess.Seed;
using ToolkitSample.Model;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class DataSeedUnitTests
    {
        [Fact]
        public void ShouldGetAllObjects()
        {
            // Arrange
            IDataSeed departmentDataSeed = new DepartmentDataSeed();

            // Act
            var allObject = departmentDataSeed.GetAllObjects();

            // Assert
            allObject.Should().NotBeNull();
            allObject.Should().HaveCount(2);

            var allDepartments = allObject.OfType<Department>();
            allDepartments.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldGetAll()
        {
            // Arrange
            var departmentDataSeed = new DepartmentDataSeed();

            // Act
            var allDepartments = departmentDataSeed.GetAll();

            // Assert
            allDepartments.Should().NotBeNull();
            allDepartments.Should().HaveCount(2);
        }
    }
}
