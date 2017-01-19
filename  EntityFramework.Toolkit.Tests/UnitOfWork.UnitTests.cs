using System;
using System.Collections.Generic;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Exceptions;

using FluentAssertions;

using Moq;

using ToolkitSample.DataAccess.Context;

using Xunit;

namespace EntityFramework.Toolkit.Tests
{
    public class UnitOfWorkUnitTests
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
            sampleContextTwoMock.Setup(m => m.SaveChanges()).Throws(new InvalidOperationException("SampleContextTwo failed to SaveChanges."));

            unitOfWork.RegisterContext(sampleContextOneMock.Object);
            unitOfWork.RegisterContext(sampleContextTwoMock.Object);

            // Act
            Action action = () => unitOfWork.Commit();

            // Assert
            var ex = action.ShouldThrow<UnitOfWorkException>();
            ex.Which.Message.Should().Contain("UnitOfWork in context 'ObjectProxy_1' failed to commit.");
            ex.WithInnerException<InvalidOperationException>();
            ex.Which.InnerException.Message.Should().Contain("SampleContextTwo failed to SaveChanges.");

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
                numberOfChanges.Should().HaveCount(0);
            }
        }

        [Fact]
        public void ShouldCommitChangesOfContext()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork();

            var contextMock = new Mock<IContext>();
            var changeSet = new ChangeSet(contextMock.GetType(), new List<IChange> { Change.CreateAddedChange(new object()) });
            contextMock.Setup(c => c.SaveChanges()).Returns(changeSet);

            unitOfWork.RegisterContext(contextMock.Object);

            // Act
            var numberOfChanges = unitOfWork.Commit();

            // Assert
            numberOfChanges.Should().HaveCount(1);
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
                    var changeSets = innerUnitofWork.Commit();

                    // Assert
                    changeSets.Should().HaveCount(0);
                }
                outerUnitOfWork.Commit();
            }
        }

        [Fact]
        public void ShouldDisposeAllRegisteredContexts()
        {
            // Arrange
            var sampleContextMock = new Mock<ISampleContext>();

            // Act
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                unitOfWork.RegisterContext(sampleContextMock.Object);
            }

            // Assert
            sampleContextMock.Verify(x => x.SaveChanges(), Times.Never);
            sampleContextMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact(Skip="TODO")]
        public void ShouldThrowObjectDisposedExceptionOnCommitAfterDispose()
        {
            // Arrange
            var sampleContextMock = new Mock<ISampleContext>();
            IUnitOfWork unitOfWork = new UnitOfWork();
            unitOfWork.RegisterContext(sampleContextMock.Object);
            unitOfWork.Dispose();

            // Act
            Action action = () => unitOfWork.Commit();

            // Assert
            action.ShouldThrow<ObjectDisposedException>();
            sampleContextMock.Verify(x => x.SaveChanges(), Times.Never);
            sampleContextMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public async void ShouldCommitAsync()
        {
            // Arrange
            var sampleContextMock = new Mock<ISampleContext>();

            // Act
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                unitOfWork.RegisterContext(sampleContextMock.Object);
                await unitOfWork.CommitAsync();
            }

            // Assert
            sampleContextMock.Verify(x => x.SaveChanges(), Times.Never);
            sampleContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            sampleContextMock.Verify(x => x.Dispose(), Times.Once);
        }

        //TODO Write test to save + check summary of changes
        //TODO Write test to saveasync + check summary of changes
    }
}