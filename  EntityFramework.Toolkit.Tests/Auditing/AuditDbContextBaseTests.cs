using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EntityFramework.Toolkit.Auditing;
using EntityFramework.Toolkit.Testing;

using FluentAssertions;

using ToolkitSample.DataAccess.Context.Auditing;
using ToolkitSample.Model;
using ToolkitSample.Model.Auditing;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests.Auditing
{
    public class AuditDbContextBaseTests : ContextTestBase<TestAuditDbContext>
    {
        private const string TestAuditUser = "TestAuditUser";

        public AuditDbContextBaseTests(ITestOutputHelper testOutputHelper)
            : base(
                dbConnectionString: () => "Data Source=(localdb)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\AuditingTestDb.mdf; Integrated Security=True;".RandomizeDatabaseName(),
                log: testOutputHelper.WriteLine)
        {
            // Set variable 'DataDirectory' to the currentDirectory
            var currentDirectory = Directory.GetCurrentDirectory();
            AppDomain.CurrentDomain.SetData("DataDirectory", currentDirectory);
        }

        [Fact]
        public async void ShouldAuditCreatedAndUpdatedDate()
        {
            // Arrange
            var initialEmployee = Stubs.Testdata.Employees.CreateEmployee1();

            // Act
            using (var auditDbContext = this.CreateContext())
            {
                auditDbContext.Set<Employee>().Add(initialEmployee);
                auditDbContext.SaveChanges();

                await Task.Delay(1000);

                initialEmployee.FirstName = "Updated " + initialEmployee.FirstName;
                auditDbContext.SaveChanges();
            }

            // Assert
            using (var auditDbContext = this.CreateContext())
            {
                var allEmployees = auditDbContext.Set<Employee>().ToList();
                allEmployees.Where(e => e.CreatedDate > DateTime.MinValue).Should().HaveCount(1);
                allEmployees.Where(e => e.UpdatedDate > e.CreatedDate).Should().HaveCount(1);
            }
        }

        [Fact]
        public void ShouldAuditPropertyUpdate()
        {
            // Arrange
            using (var context = this.CreateContext())
            {
                context.Employees.Add(Stubs.Testdata.Employees.CreateEmployee1());
                context.SaveChanges();
            }

            // Act
            using (var context = this.CreateContext())
            {
                var customer = context.Employees.Find(1);
                for (var i = 0; i < 10; i++)
                {
                    customer.FirstName = customer.FirstName + " " + i;
                    context.SaveChanges();
                }
            }

            // Assert
            using (var context = this.CreateContext())
            {
                var customer = context.Employees.Find(1);
                customer.FirstName.Should().Be("Thomas 0 1 2 3 4 5 6 7 8 9");

                var employeeAudits = context.EmployeeAudits.ToList();
                employeeAudits.Should().HaveCount(11);
                employeeAudits.Where(a => a.AuditType == AuditEntityState.Added).Should().HaveCount(1);
                employeeAudits.Where(a => a.AuditType == AuditEntityState.Modified).Should().HaveCount(10);
            }
        }

        [Fact]
        public void ShouldAuditDelete()
        {
            // Arrange
            using (var context = this.CreateContext())
            {
                context.Employees.Add(Stubs.Testdata.Employees.CreateEmployee1());
                context.SaveChanges();
            }

            // Act
            using (var context = this.CreateContext())
            {
                var customer = context.Employees.Find(1);
                context.Delete(customer);
                context.SaveChanges();
            }

            // Assert
            using (var context = this.CreateContext())
            {
                var customer = context.Employees.Find(1);
                customer.Should().BeNull();

                var employeeAudits = context.EmployeeAudits.ToList();
                employeeAudits.Should().HaveCount(2);
                employeeAudits.Where(a => a.AuditType == AuditEntityState.Added).Should().HaveCount(1);
                employeeAudits.Where(a => a.AuditType == AuditEntityState.Deleted).Should().HaveCount(1);
            }
        }

        [Fact]
        public void ProxiesTrueTest()
        {
            // Arrange
            var testAuditDbContext = this.CreateContext();

            // Act
            testAuditDbContext.Configuration.ProxyCreationEnabled = true;

            // Assert
            testAuditDbContext.Proxies.Should().BeTrue();
        }

        [Fact]
        public void ShouldRollbackAuditIfSaveChangesFails()
        {
            // Arrange
            var initialEmployee = Stubs.Testdata.Employees.CreateEmployee1();

            string firstNameChange1 = initialEmployee.FirstName + " from employeeContext1";
            string firstNameChange2 = initialEmployee.FirstName + " from employeeContext2";

            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Set<Employee>().Add(initialEmployee);
                employeeContext.SaveChanges();
            }

            // Act
            using (var employeeContext1 = this.CreateContext())
            {
                // Get an employee (which has a Version Timestamp) in one context and modify
                var employeeFromContext1 = employeeContext1.Set<Employee>().First(p => p.Id == 1);
                employeeFromContext1.FirstName = firstNameChange1;

                // Modify and Save the same employee in another context to simulate concurrent access
                using (var employeeContext2 = this.CreateContext())
                {
                    var employeeFromContext2 = employeeContext2.Set<Employee>().First(p => p.Id == 1);
                    employeeFromContext2.FirstName = firstNameChange2;
                    employeeContext2.SaveChanges();
                }

                // SaveChanges of the first context results in a DbUpdateConcurrencyException
                Action action = () => employeeContext1.SaveChanges();

                // Assert
                action.ShouldThrow<DbUpdateConcurrencyException>();

                using (var employeeContext = this.CreateContext())
                {
                    var employeeAudits = employeeContext.EmployeeAudits.ToList();
                    employeeAudits.Should().HaveCount(2);
                    employeeAudits.Where(a => a.AuditType == AuditEntityState.Added).Should().HaveCount(1);
                    employeeAudits.Where(a => a.AuditType == AuditEntityState.Modified).Should().HaveCount(1);

                    var employeeAuditAdded = employeeAudits.Single(a => a.AuditType == AuditEntityState.Added);
                    employeeAuditAdded.Id.Should().Be(0);
                    employeeAuditAdded.FirstName.Should().Be(initialEmployee.FirstName);
                    employeeAuditAdded.LastName.Should().Be(initialEmployee.LastName);

                    var employeeAuditModified = employeeAudits.Single(a => a.AuditType == AuditEntityState.Modified);
                    employeeAuditModified.Id.Should().Be(initialEmployee.Id);
                    employeeAuditModified.FirstName.Should().Be(initialEmployee.FirstName);
                    employeeAuditModified.LastName.Should().Be(initialEmployee.LastName);
                }
            }

            // Assert

        }

        ////[Fact]
        ////public void UpdateTestNoProxy()
        ////{
        ////    var c = CreateContextInstance(typeof(Context));

        ////    //var x  = Constructor<Func<string, string, Context>>.Ctor("test", "test2");

        ////    Context context = this.CreateContext();
        ////    this.UpdateTest(context);
        ////}

        ////[Fact]
        ////public void UpdateTestProxy()
        ////{
        ////    Context context = this.CreateContext();
        ////    this.UpdateTest(context);
        ////}

        ////private void UpdateTest(Context context)
        ////{
        ////    string FirstName = "Updated";
        ////    DateTime updated = DateTime.Now;

        ////    DbConnection conn = this.GetConnection();

        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = @"INSERT INTO Customers(CustomerId, FirstName,Updated,UpdateUser) VALUES (1, 'Unit Test', '2012-01-01 12:00:00', 'UnitTest')";
        ////        cmd.ExecuteNonQuery();
        ////    }

        ////    Customer customer = context.Customers.Find(1);
        ////    customer.FirstName = FirstName;
        ////    context.SaveChanges(user);

        ////    // Chech the audit records has been created.
        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = "select * from customeraudits";
        ////        using (var r = cmd.ExecuteReader())
        ////        {
        ////            int records = 0;
        ////            while (r.Read())
        ////            {
        ////                records++;
        ////            }
        ////            Assert.Equal(1, records);
        ////        }
        ////    }

        ////    // Check the audit fields.
        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = "select * from customeraudits";
        ////        using (var r = cmd.ExecuteReader())
        ////        {
        ////            r.Read();
        ////            Assert.Equal(1, r["CustomerAuditId"]);
        ////            Assert.Equal(1, r["CustomerId"]);
        ////            Assert.Equal(user, r["UpdateUser"]);
        ////            //Assert.Equal(updated, r["Updated"]);
        ////            Assert.Equal("Unit Test", r["FirstName"]);
        ////            Assert.Equal(user, r["AuditUser"]);
        ////            Assert.Equal(0, r["AuditType"]);
        ////        }
        ////    }
        ////}

        ////[Fact]
        ////public void DeleteTestNoProxy()
        ////{
        ////    Context context = this.CreateContext();
        ////    this.DeleteTest(context);
        ////}

        ////[Fact]
        ////public void DeleteTestProxy()
        ////{
        ////    Context context = this.CreateContext();
        ////    this.DeleteTest(context);
        ////}

        ////private void DeleteTest(Context context)
        ////{

        ////    Customer customer = context.Customers.Find(1);
        ////    context.Customers.Remove(customer);
        ////    context.SaveChanges(user);

        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = "select * from customers";
        ////        using (var r = cmd.ExecuteReader())
        ////        {
        ////            int records = 0;
        ////            while (r.Read())
        ////            {
        ////                records++;
        ////            }
        ////            Assert.Equal(0, records);
        ////        }
        ////    }

        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = "select * from customeraudits";
        ////        using (var r = cmd.ExecuteReader())
        ////        {
        ////            int records = 0;
        ////            while (r.Read())
        ////            {
        ////                records++;
        ////            }
        ////            Assert.Equal(1, records);
        ////        }
        ////    }

        ////    using (DbCommand cmd = conn.CreateCommand())
        ////    {
        ////        cmd.CommandText = "select * from customeraudits";
        ////        using (var r = cmd.ExecuteReader())
        ////        {
        ////            r.Read();
        ////            Assert.Equal(user, r["AuditUser"]);
        ////            Assert.Equal(1, r["AuditType"]);
        ////        }
        ////    }
        ////}

        [Fact]
        public void ShouldRegisterAuditType()
        {
            // Arrange
            var auditTypeInfo = new AuditTypeInfo(typeof(TestEntity), typeof(TestEntityAudit));
            var context = this.CreateContext();

            // Act
            context.RegisterAuditType(auditTypeInfo);

            // Assert
            context.TestEntities.Add(new TestEntity());
            context.SaveChanges(TestAuditUser);

            var testEntityAudits = context.TestEntityAudits.ToList();
            testEntityAudits.Should().HaveCount(1);
            testEntityAudits.ElementAt(0).AuditType.Should().Be(AuditEntityState.Added);
        }

        [Fact]
        public void ShouldFailToRegisterAuditTypeTwice()
        {
            // Arrange
            var auditTypeInfo = new AuditTypeInfo(typeof(TestEntity), typeof(TestEntityAudit));
            var context = this.CreateContext();
            context.RegisterAuditType(auditTypeInfo);

            // Act
            Action action = () => context.RegisterAuditType(auditTypeInfo);

            // Assert
            action.ShouldThrow<ArgumentException>().Which.Message.Should().Contain("Type TestEntity is already registered for auditing.");
        }
    }
}