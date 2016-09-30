using EntityFramework.Toolkit.Testing;

using ToolkitSample.DataAccess.Context;

using Xunit;
using Xunit.Abstractions;

namespace EntityFramework.Toolkit.Tests
{
    public class EdmxToolsTests : ContextTestBase<EmployeeContext>
    {
        private readonly string basePath = @"..\..\..\Samples\ToolkitSample.DataAccess\" + typeof(EmployeeContext).Name;
        private readonly ITestOutputHelper testOutputHelper;

        public EdmxToolsTests(ITestOutputHelper testOutputHelper)
            : base(dbConnection: () => new EmployeeContextTestDbConnection())
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WriteEdmxFileTest()
        {
            EdmxTools.UpdateEdmx(this.CreateContext(), this.basePath + ".edmx");
        }

        [Fact]
        public void CreateDatabaseScriptTest()
        {
            EdmxTools.CreateDatabaseScript(this.CreateContext(), this.basePath + ".sql");
        }
    }
}