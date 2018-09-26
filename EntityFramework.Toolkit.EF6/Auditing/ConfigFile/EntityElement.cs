using System.Configuration;

namespace EntityFramework.Toolkit.EF6.Auditing.ConfigFile
{
    internal class EntityElement : ConfigurationElement
    {
        private const string EntityTypeElementName = "entity";
        private const string AuditEntityTypeElementName = "auditEntity";

        [ConfigurationProperty(EntityTypeElementName, IsRequired = false)]
        internal string EntityType
        {
            get { return (string) this[EntityTypeElementName]; }

            set { this[EntityTypeElementName] = value; }
        }

        [ConfigurationProperty(AuditEntityTypeElementName, IsRequired = false)]
        internal string AuditEntityType
        {
            get { return (string) this[AuditEntityTypeElementName]; }

            set { this[AuditEntityTypeElementName] = value; }
        }
    }
}