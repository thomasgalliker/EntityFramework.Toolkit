using EntityFramework.Toolkit.EF6.Contracts.Auditing;

namespace ToolkitSample.Model.Auditing
{
    public class EmployeeAudit : AuditEntity
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }
    }
}