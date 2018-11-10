using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Extensions;
using FluentAssertions;

using ToolkitSample.DataAccess.Context.Auditing;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void ShouldGetFormattedName()
        {
            // Arrange
            var type = typeof(Func<bool, string, int>);

            // Act
            var formattedName = type.GetFormattedName();

            // Assert
            formattedName.Should().Be("Func<Boolean, String, Int32>");
        }

        [Fact]
        public void ShouldGetFormattedFullName()
        {
            // Arrange
            var type = typeof(IList<string>);

            // Act
            var formattedName = type.GetFormattedFullname();

            // Assert
            formattedName.Should().Be("System.Collections.Generic.IList<System.String>");
        }

        [Fact]
        public void ShouldThrowExceptionIfCouldNotFindMatchingConstructorWithNoParameters()
        {
            // Arrange
            var args = new object[] { };

            // Act
            Action action = () => typeof(TestAuditDbContext).GetMatchingConstructor(args);

            // Assert
            action.ShouldThrow<InvalidOperationException>().Which.Message.Should().Contain("TestAuditDbContext does not have a constructor with no parameters.");
        }

        [Fact]
        public void ShouldThrowExceptionIfCouldNotFindMatchingConstructorWithParameters()
        {
            // Arrange
            var args = new object[] { 111f, 222m };

            // Act
            Action action = () => typeof(TestAuditDbContext).GetMatchingConstructor(args);

            // Assert
            action.ShouldThrow<InvalidOperationException>().Which.Message.Should().Contain("TestAuditDbContext does not have a constructor with parameters (Single, Decimal).");
        }

        [Fact]
        public void ShouldGetConstructorWithOptionalParameter()
        {
            // Arrange
            var args = new object[]
            {
                new DbConnection("Data Source=(localdb)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\AuditingTestDb.mdf; Integrated Security=True;"),
                new DropCreateDatabaseAlways<TestAuditDbContext>()
            };

            // Act
            var contextCtor = typeof(TestAuditDbContext).GetMatchingConstructor(args);

            // Assert
            var contextCtorParameters = contextCtor.ConstructorInfo.GetParameters();
            contextCtorParameters.Should().HaveCount(3);
            contextCtorParameters.ElementAt(0).ParameterType.Should().Be(typeof(IDbConnection));
            contextCtorParameters.ElementAt(1).ParameterType.Should().Be(typeof(IDatabaseInitializer<TestAuditDbContext>));
            contextCtorParameters.ElementAt(2).ParameterType.Should().Be(typeof(Action<string>));

            var testContext = contextCtor.Invoke();
            testContext.Should().BeOfType<TestAuditDbContext>();
        }
    }
}