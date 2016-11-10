
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using EntityFramework.Toolkit.Utils;

using FluentAssertions;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Utils
{
    public class DbHelpersTests
    {
        [Fact]
        public void ShouldParseStringMemberExpression()
        {
            // Arrange
            Expression<Func<Product, string>> expr = p => p.StringProperty;
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("StringProperty");
        }

        [Fact]
        public void ShouldParseObjectMemberExpression()
        {
            // Arrange
            Expression<Func<Product, object>> expr = p => p.BaseProperty;
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("BaseProperty");
        }

        [Fact]
        public void ShouldParseSelectCollectionMemberExpression()
        {
            // Arrange
            Expression<Func<Product, object>> expr = p => p.CollectionProperty.Select(x => x.Key);
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("CollectionProperty.Key");
        }
    }

    public class DerivedClass : BaseClass
    {
        public object ObjectProperty { get; set; }
    }

    public class BaseClass
    {
    }

    public class Product
    {
        public string StringProperty { get; set; }

        public BaseClass BaseProperty { get; set; }

        public ICollection<KeyValuePair<int, string>> CollectionProperty { get; set; }
    }
}
