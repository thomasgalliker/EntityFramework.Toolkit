using EntityFramework.Toolkit.Auditing;
using ToolkitSample.Model.Auditing;

namespace ToolkitSample.DataAccess.Context
{
    public class TestEntityAuditEntityTypeConfiguration : AuditEntityTypeConfiguration<TestEntityAudit, int>
    {
        public TestEntityAuditEntityTypeConfiguration()
        {
            this.Property(e => e.TestEntityId).IsRequired();
        }
    }
}