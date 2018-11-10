using System.Data.Entity.ModelConfiguration;
using EntityFramework.Toolkit.Core.Auditing;

namespace EntityFramework.Toolkit.Auditing
{
    public abstract class AuditEntityTypeConfiguration<TAuditEntity, TAuditKey> :
        EntityTypeConfiguration<TAuditEntity> where TAuditEntity : class,
        IAuditEntity<TAuditKey>
    {
        protected AuditEntityTypeConfiguration()
        {
            this.HasKey(e => e.AuditId);
            this.Property(e => e.AuditDate).IsRequired();
            this.Property(e => e.AuditUser).IsRequired();
            this.Property(e => e.AuditType).IsRequired();
        }
    }
}