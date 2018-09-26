using System.Data.Entity;
using EntityFramework.Toolkit.EF6;
using EntityFramework.Toolkit.EF6.Contracts;
using EntityFramework.Toolkit.EF6.Testing;
using FluentAssertions;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class ContextTestBaseTests_DbConnectionStringOnly : ContextTestBase<ContextTestBaseTests_DbConnectionStringOnly.TestContext>
    {
        public ContextTestBaseTests_DbConnectionStringOnly()
            : base(dbConnectionString: () => @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EF.Toolkit.ContextTestBaseTests.mdf; Integrated Security=True;".RandomizeDatabaseName())
        {
        }

        [Fact]
        public void ShouldCreateContextWithCtorParameters()
        {
            // Arrange

            // Act
            var testContext = this.CreateContext();

            // Assert
            testContext.Should().BeOfType<TestContext>();
        }

        public class TestContext : DbContextBase<TestContext>
        {
            public TestContext(string nameOrConnectionString, IDatabaseInitializer<TestContext> databaseInitializer)
                : base(nameOrConnectionString, databaseInitializer)
            {
            }
        }
    }

    public class ContextTestBaseTests_DbConnectionOnly : ContextTestBase<ContextTestBaseTests_DbConnectionOnly.TestContext>
    {
        public ContextTestBaseTests_DbConnectionOnly()
            : base(dbConnection: () => new DbConnection(@"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EF.Toolkit.ContextTestBaseTests.mdf; Integrated Security=True;".RandomizeDatabaseName()))
        {
        }

        [Fact]
        public void ShouldCreateContextWithCtorParameters()
        {
            // Arrange

            // Act
            var testContext = this.CreateContext();

            // Assert
            testContext.Should().BeOfType<TestContext>();
        }

        public class TestContext : DbContextBase<TestContext>
        {
            public TestContext(IDbConnection dbConnection, IDatabaseInitializer<TestContext> databaseInitializer)
                : base(dbConnection, databaseInitializer)
            {
            }
        }
    }
}