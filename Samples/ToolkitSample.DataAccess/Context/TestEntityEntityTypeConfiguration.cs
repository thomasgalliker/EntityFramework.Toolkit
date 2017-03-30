using System.Data.Entity.ModelConfiguration;

using ToolkitSample.Model.Auditing;

namespace ToolkitSample.DataAccess.Context
{
    public class TestEntityEntityTypeConfiguration : EntityTypeConfiguration<TestEntity>
    {
        public TestEntityEntityTypeConfiguration()
        {
            this.HasKey(e => e.TestEntityId);
        }
    }
}