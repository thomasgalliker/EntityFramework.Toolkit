using System.Collections.Generic;

namespace EntityFramework.Toolkit.Auditing
{
    public class AuditDbContextConfiguration
    {
        public AuditDbContextConfiguration(bool auditEnabled, params AuditTypeInfo[] auditTypeInfos)
        {
            this.AuditEnabled = auditEnabled;
            this.AuditTypeInfos = auditTypeInfos;
        }

        public bool AuditEnabled { get; }

        public AuditTypeInfo[] AuditTypeInfos { get; }
    }
}