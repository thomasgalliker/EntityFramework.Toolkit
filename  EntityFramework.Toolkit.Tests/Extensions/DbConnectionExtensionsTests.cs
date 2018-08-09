using Dwh.Testing;
using EntityFramework.Toolkit.Testing;
using FluentAssertions;
using ToolkitSample.DataAccess.Context;
using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests.Extensions
{
    public class DbConnectionExtensionsTests : ContextTestBase<EmployeeContext>
    {
        private static readonly IDbConnection dbConnection = new EmployeeContextTestDbConnection();

        public DbConnectionExtensionsTests(ITestOutputHelper testOutputHelper)
            : base(() => dbConnection, null, testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldDropDatabaseUsingDbConnection()
        {
            // Arrange
            using (var context = CreateContext())
            {
                context.Database.CreateIfNotExists();
            }

            // Act
            dbConnection.DropDatabase();

            // Assert
        }

        [Fact]
        public void ShouldGetDatabaseName()
        {
            // Arrange
            using (var context = CreateContext())
            {
                context.Database.CreateIfNotExists();
            }

            // Act
            var databaseName = dbConnection.GetDatabaseName();

            // Assert
            databaseName.Should().StartWith("EF.Toolkit.Tests_");
            databaseName.Should().EndWith(".mdf");
        }
    }
}