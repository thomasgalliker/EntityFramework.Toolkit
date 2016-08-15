
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
            Expression<Func<BaseClass, string>> expr = bond => bond.StringProperty;
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
            Expression<Func<DerivedClass, object>> expr = bond => bond.ObjectProperty;
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("ObjectProperty");
        }

        [Fact]
        public void ShouldParseSelectCollectionMemberExpression()
        {
            // Arrange
            Expression<Func<BaseClass, object>> expr = bond => bond.CollectionProperty.Select(x => x.Key);
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("CollectionProperty.Key");
        }

        [Fact]
        public void ShouldParseCastAsMemberExpression()
        {
            // Arrange
            Expression<Func<BaseClass, object>> expr = bond => bond.As<DerivedClass>().ObjectProperty;
            string path;

            // Act
            var isParsed = DbHelpers.TryParsePath(expr.Body, out path);

            // Assert
            isParsed.Should().BeTrue();
            path.Should().Be("ObjectProperty");
        }
    }

    public class DerivedClass : BaseClass
    {
        public object ObjectProperty { get; set; }
    }

    public class BaseClass
    {
        public string StringProperty { get; set; }

        public ICollection<KeyValuePair<int, string>> CollectionProperty { get; set; }
    }
}
