using EntityFramework.Toolkit.Testing;

using ToolkitSample.DataAccess.Context;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class EdmxTests : ContextTestBase<EmployeeContext>
    {
        private readonly string basePath = @"..\..\..\Samples\ToolkitSample.DataAccess\" + typeof(EmployeeContext).Name;
        private readonly ITestOutputHelper testOutputHelper;

        public EdmxTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: new EmployeeContextTestDbConnection())
        {
            this.testOutputHelper = testOutputHelper;
        }



        [Fact]
        public void WriteEdmxFileTest()
        {
            EdmxTools.UpdateEdmx(this.Context, this.basePath + ".edmx");
        }

        [Fact]
        public void CreateDatabaseScriptTest()
        {
            EdmxTools.CreateDatabaseScript(this.Context, this.basePath + ".sql");
        }
    }
}