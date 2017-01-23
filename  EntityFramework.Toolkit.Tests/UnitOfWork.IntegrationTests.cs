using System;
using System.Data.Entity;
using System.Linq;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Exceptions;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;

using FluentAssertions;

using Moq;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class UnitOfWorkIntegrationTests : ContextTestBase<EmployeeContext>
    {
        public UnitOfWorkIntegrationTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(),
                  databaseInitializer: null,
                  log: testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldFailToCommitMultipleContexts2()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork();

            var context1 = this.CreateContext(new DropCreateDatabaseAlways<EmployeeContext>());
            var context2 = new Mock<ISampleContextTwo>();
            context2.Setup(m => m.SaveChanges()).Throws(new InvalidOperationException("SampleContextTwo failed to SaveChanges."));

            context1.Set<Employee>().Add(Testdata.Employees.CreateEmployee1());

            unitOfWork.RegisterContext(context1);
            unitOfWork.RegisterContext(context2.Object);

            // Act
            Action action = () => unitOfWork.Commit();

            // Assert
            var ex = action.ShouldThrow<UnitOfWorkException>();
            ex.Which.Message.Should().Contain("failed to commit.");
            ex.WithInnerException<InvalidOperationException>();
            ex.Which.InnerException.Message.Should().Contain("SampleContextTwo failed to SaveChanges.");

            var context = this.CreateContext(new DropCreateDatabaseAlways<EmployeeContext>());
            context.Set<Employee>().ToList().Should().HaveCount(0);
        }

        //TODO Write test to save + check summary of changes
        //TODO Write test to saveasync + check summary of changes
    }
}