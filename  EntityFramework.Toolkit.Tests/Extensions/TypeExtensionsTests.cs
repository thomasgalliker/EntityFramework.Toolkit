using System;
using System.Collections.Generic;

using FluentAssertions;

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
    }
}
