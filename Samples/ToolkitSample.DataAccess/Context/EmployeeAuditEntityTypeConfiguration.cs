using EntityFramework.Toolkit.Auditing;
using ToolkitSample.Model.Auditing;

namespace ToolkitSample.DataAccess.Context
{
    public class EmployeeAuditEntityTypeConfiguration : AuditEntityTypeConfiguration<EmployeeAudit, int>
    {
        public EmployeeAuditEntityTypeConfiguration()
        {
            this.Property(e => e.Id).IsRequired();
            this.Property(e => e.LastName).IsRequired();
            this.Property(e => e.FirstName).IsRequired();

            this.ToTable(nameof(EmployeeAudit));
        }
    }
}