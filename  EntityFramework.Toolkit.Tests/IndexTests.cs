using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Extensions;

using FluentAssertions;

using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class IndexTests : ContextTestBase<EmployeeContext>
    {
        public IndexTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection(), 
                  initializer: new CreateDatabaseIfNotExists<EmployeeContext>(), 
                  log: testOutputHelper.WriteLine)
        {
        }

        [Fact]
        public void ShouldAddRoomsIfMultipleColumnsAreUnique()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Level = 1, Sector = "A" },
                new Room { Level = 1, Sector = "B" },
                new Room { Level = 1, Sector = "C" },
            };
            ChangeSet committedChangeSet;

            // Act
            using (var context = this.CreateContext())
            {
                context.Set<Room>().Add(rooms[0]);
                context.Set<Room>().Add(rooms[1]);
                context.Set<Room>().Add(rooms[2]);
                committedChangeSet = context.SaveChanges();
            }

            // Assert
            committedChangeSet.Assert(expectedNumberOfAdded: 3, expectedNumberOfModified: 0, expectedNumberOfDeleted: 0);

            using (var context = this.CreateContext())
            {
                var allRooms = context.Set<Room>().ToList();
                allRooms.Should().HaveCount(3);
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfIfMultipleColumnsAreNotUnique()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Level = 1, Sector = "A" },
                new Room { Level = 1, Sector = "A" },
            };

            // Act
            using (var context = this.CreateContext())
            {
                context.Set<Room>().Add(rooms[0]);
                context.Set<Room>().Add(rooms[1]);

                Action action = () => context.SaveChanges();

                // Assert
                var ex = action.ShouldThrow<DbUpdateException>();
                ex.Which.InnerException.InnerException.Message.Should()
                    .Contain("Cannot insert duplicate key row in object 'dbo.Room' with unique index 'UQ_Level_Sector'. The duplicate key value is (1, A).");
            }
        }
    }
}