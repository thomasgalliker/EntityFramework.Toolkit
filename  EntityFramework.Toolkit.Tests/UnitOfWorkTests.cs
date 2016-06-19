using System;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Exceptions;

using FluentAssertions;

using Moq;

using ToolkitSample.DataAccess.Context;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void ShouldCommitToSingleContext()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            var sampleContextMock = new Mock<ISampleContext>();

            unitOfWork.RegisterContext(sampleContextMock.Object);

            // Act
            unitOfWork.Commit();

            // Assert
            sampleContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ShouldCommitToMultipleContexts()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            var sampleContextOneMock = new Mock<ISampleContext>();
            var sampleContextTwoMock = new Mock<ISampleContextTwo>();

            unitOfWork.RegisterContext(sampleContextOneMock.Object);
            unitOfWork.RegisterContext(sampleContextTwoMock.Object);

            // Act
            unitOfWork.Commit();

            // Assert
            sampleContextOneMock.Verify(x => x.SaveChanges(), Times.Once);
            sampleContextTwoMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ShouldFailToCommitMultipleContexts()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();
            var sampleContextOneMock = new Mock<ISampleContext>();
            var sampleContextTwoMock = new Mock<ISampleContextTwo>();
            sampleContextTwoMock.Setup(m => m.SaveChanges()).Throws(new InvalidOperationException("SampleContextOne failed to SaveChanges."));

            unitOfWork.RegisterContext(sampleContextOneMock.Object);
            unitOfWork.RegisterContext(sampleContextTwoMock.Object);

            // Act
            Action action = () => unitOfWork.Commit();

            // Assert
            action.ShouldThrow<UnitOfWorkException>().WithInnerException<InvalidOperationException>();
            sampleContextOneMock.Verify(x => x.SaveChanges(), Times.Once);
            sampleContextTwoMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ShouldCommitNoChangesWhenNothingNeedsToBeDone()
        {
            // Arrange
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                // Act
                var numberOfChanges = unitOfWork.Commit();

                // Assert
                numberOfChanges.Should().Be(0);
            }
        }

        [Fact]
        public void ShouldCommitChangesOfContext()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork();
            var contextMock = new Mock<IContext>();
            contextMock.Setup(c => c.SaveChanges()).Returns(99);
            unitOfWork.RegisterContext(contextMock.Object);

            // Act
            var numberOfChanges = unitOfWork.Commit();

            // Assert
            numberOfChanges.Should().Be(99);
        }

        [Fact]
        public void ShouldHandleMultipleInstances()
        {
            // Arrange
            using (IUnitOfWork outerUnitOfWork = new UnitOfWork())
            {
                using (IUnitOfWork innerUnitofWork = new UnitOfWork())
                {
                    // Act
                    var numberOfChanges = innerUnitofWork.Commit();

                    // Assert
                    numberOfChanges.Should().Be(0);
                }
                outerUnitOfWork.Commit();
            }
        }
    }
}
