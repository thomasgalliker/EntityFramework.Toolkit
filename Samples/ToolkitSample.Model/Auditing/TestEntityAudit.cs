using System;
using EntityFramework.Toolkit.EF6.Contracts.Auditing;

namespace ToolkitSample.Model.Auditing
{
    public class TestEntityAudit : IAuditEntity<int>
    {
        public int AuditId { get; set; }

        public DateTime? Updated { get; set; }

        public string UpdateUser { get; set; }

        public DateTime AuditDate { get; set; }

        public string AuditUser { get; set; }

        public AuditEntityState AuditType { get; set; }

        public int TestEntityId { get; set; }
    }
}
