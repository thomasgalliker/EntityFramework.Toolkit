using System.Reflection;

using Autofac;

using FluentAssertions;

using ToolkitSample.DataAccess.Contracts.Repository;
using ToolkitSample.DataAccess.Repository;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Modularity
{
    public class DataAccessModuleTests
    {
        [Fact]
        public void ShouldBuildAndResolveDependencies()
        {
            // Arrange
            var container = GetContainer();

            // Act
            var employeeRepository = container.Resolve<IEmployeeRepository>();

            // Assert
            employeeRepository.Should().NotBeNull();
            employeeRepository.Should().BeOfType<EmployeeRepository>();
        }

        private static IContainer GetContainer()
        {
            var container = new Autofac.ContainerBuilder();
            container.RegisterAssemblyModules(Assembly.Load("ToolkitSample.DataAccess"));
            return container.Build();
        }

    }
}
