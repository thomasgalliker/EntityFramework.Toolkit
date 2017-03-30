using System;

using EntityFramework.Toolkit.Auditing;

using FluentAssertions;

using ToolkitSample.Model.Auditing;

using Xunit;

namespace EntityFramework.Toolkit.Tests.Auditing
{
    public class AuditTypeInfoTests
    {
        [Fact]
        public void ShouldSuccessfullyCreateAuditTypeInfo()
        {
            // Arrange
            var auditableEntityType = typeof(TestEntity);
            var auditEntityType = typeof(TestEntityAudit);

            // Act
            var auditTypeInfo = new AuditTypeInfo(auditableEntityType, auditEntityType);

            // Assert
            auditTypeInfo.Should().NotBeNull();
        }

        [Fact]
        public void ShouldFailToCreateAuditTypeInfoIfIAuditEntityIsNotImplemented()
        {
            // Arrange
            var auditableEntityType = typeof(TestEntity);
            var auditEntityType = typeof(TestInvalidAuditEntity);

            // Act
            Action action = () => new AuditTypeInfo(auditableEntityType, auditEntityType);

            // Assert
            action.ShouldThrow<ArgumentException>()
                .Which.Message.Should()
                .Contain("Entity of type TestInvalidAuditEntity does implement IAuditEntity.");
        }
    }
}
