using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using EntityFramework.Toolkit.Testing;
using EntityFramework.Toolkit.Tests.Stubs;
using FluentAssertions;
using Tanneryd.BulkOperations.EF6;
using Tanneryd.BulkOperations.EF6.Model;
using ToolkitSample.DataAccess.Context;
using ToolkitSample.Model;
using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    [Collection("PeformanceTests")]
    public class PerformanceTests : ContextTestBase<EmployeeContext>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public PerformanceTests(ITestOutputHelper testOutputHelper)
            : base(() => new EmployeeContextTestDbConnection(),
                new DropCreateDatabaseAlways<EmployeeContext>(),
                testOutputHelper.WriteLine)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(PerformanceTestData))]
        public void AddEntitiesPerformanceTest_AddSingleEntities(int count)
        {
            // Arrange
            using (var employeeContext = CreateContext())
            {
                employeeContext.Database.CreateIfNotExists();
            }

            var employeesTestData = Testdata.Employees.CreateEmployees(count).ToList();

            // Act
            var stopwatch = new Stopwatch();
            using (var employeeContext = CreateContext())
            {
                stopwatch.Start();
                foreach (var employee in employeesTestData)
                {
                }

                employeeContext.SaveChanges();
                stopwatch.Stop();
            }

            // Assert
            testOutputHelper.WriteLine($"count: {count}");
            testOutputHelper.WriteLine($"stopwatch: {stopwatch.Elapsed.TotalSeconds:F2}s");

            using (var employeeContext = CreateContext())
            {
                var employees = employeeContext.Set<Employee>().ToList();
                employees.Should().HaveCount(count);
            }
        }

        [Theory]
        [ClassData(typeof(PerformanceTestData))]
        public void AddEntitiesPerformanceTest_AddRange(int count)
        {
            // Arrange
            using (var employeeContext = CreateContext())
            {
                employeeContext.Database.CreateIfNotExists();
            }

            var employeesTestData = Testdata.Employees.CreateEmployees(count).ToList();
            //var chunks = employeesTestData.Chunk(30);

            // Act
            var stopwatch = new Stopwatch();
            using (var employeeContext = CreateContext())
            {
                stopwatch.Start();

                //foreach (var chunk in chunks)
                //{
                //    employeeContext.Set<Employee>().AddRange(chunk);
                //    employeeContext.SaveChanges();
                //}
  
                employeeContext.Set<Employee>().AddRange(employeesTestData);
                employeeContext.SaveChanges();
                stopwatch.Stop();
            }

            // Assert
            testOutputHelper.WriteLine($"count: {count}");
            testOutputHelper.WriteLine($"stopwatch: {stopwatch.Elapsed.TotalSeconds:F2}s");

            using (var employeeContext = CreateContext())
            {
                var employees = employeeContext.Set<Employee>().ToList();
                employees.Should().HaveCount(count);
            }
        }

        [Theory]
        [ClassData(typeof(PerformanceTestData))]
        public void AddEntitiesPerformanceTest_BulkInsert(int count)
        {
            // Arrange
            using (var employeeContext = this.CreateContext())
            {
                employeeContext.Database.CreateIfNotExists();
            }

            var employeesTestData = Testdata.Employees.CreateEmployees(count).ToList();
            foreach (var employee in employeesTestData)
            {
                employee.CreatedDate = DateTime.UtcNow;
                employee.UpdatedDate = null;
            }

            var department = Testdata.Departments.CreateDepartmentHumanResources();
            department.Leader = Testdata.Employees.CreateEmployee1();
            department.Leader.CreatedDate = DateTime.UtcNow;
            department.Leader.UpdatedDate = null;
            department.Employees = employeesTestData;

            // Act
            var stopwatch = new Stopwatch();
            using (var employeeContext = this.CreateContext())
            {
                stopwatch.Start();

                using (var transaction = employeeContext.Database.BeginTransaction())
                {
                    var insertRequest = new BulkInsertRequest<Employee>
                    {
                        Recursive = true,
                        Entities = employeesTestData,
                        Transaction = (SqlTransaction)transaction.UnderlyingTransaction
                    };
                    employeeContext.BulkInsertAll(insertRequest);

                    transaction.Commit();
                }
                  
                stopwatch.Stop();
            }

            // Assert
            using (var employeeContext = this.CreateContext())
            {
                var all = employeeContext.Set<Employee>().ToList();
                all.Should().HaveCount(count + 1);

                var departments = employeeContext.Set<Department>().ToList();
                departments.Should().HaveCount(1);
            }

            this.testOutputHelper.WriteLine($"count: {count}");
            this.testOutputHelper.WriteLine($"stopwatch: {stopwatch.Elapsed.TotalSeconds:F2}s");
        }

        public class PerformanceTestData : TheoryData<int>
        {
            public PerformanceTestData()
            {
                this.Add(1000);
                this.Add(10000);
            }
        }
    }

    internal static class ListExtensions
    {
        public static List<List<T>> Chunk<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}