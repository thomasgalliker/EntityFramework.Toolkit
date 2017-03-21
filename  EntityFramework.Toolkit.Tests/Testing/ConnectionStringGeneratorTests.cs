using System;
using System.Data.SqlClient;

using EntityFramework.Toolkit.Testing;

using FluentAssertions;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Testing
{
    public class ConnectionStringGeneratorTests
    {
        [Fact]
        public void ShouldReturnEmptyIfStringEmpty()
        {
            // Arrange
            var connectionString = "";
            var randomTokenLength = 5;

            // Act
            var randomConnectionString = connectionString.RandomizeDatabaseName(randomTokenLength);

            // Assert
            randomConnectionString.Should().BeEmpty();
        }

        [Fact]
        public void ShouldReturnRandomizedConnectionString()
        {
            // Arrange
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EF.Toolkit.ContextTestBaseTests.mdf; Integrated Security=True;";
            for (int randomTokenLength = 0; randomTokenLength <= 32; randomTokenLength++)
            {
                // Act
                var randomConnectionString = connectionString.RandomizeDatabaseName(randomTokenLength);

                // Assert
                var connectionStringBuilder = new SqlConnectionStringBuilder(randomConnectionString);
                connectionStringBuilder.AttachDBFilename.Should().HaveLength(@"|DataDirectory|\EF.Toolkit.ContextTestBaseTests.mdf".Length + randomTokenLength + 1);
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfRandomTokenLengthIsGreaterThan32()
        {
            // Arrange
            var randomTokenLength = 33;
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EF.Toolkit.ContextTestBaseTests.mdf; Integrated Security=True;";
            
            // Act
            Action action = () => connectionString.RandomizeDatabaseName(randomTokenLength);

            // Assert
            action.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("randomTokenLength");
        }
    }
}