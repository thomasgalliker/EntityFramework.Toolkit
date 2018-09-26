using System;
using System.Collections.Generic;
using System.Configuration;
using EntityFramework.Toolkit.EF6.Auditing.ConfigFile;

namespace EntityFramework.Toolkit.EF6.Auditing
{
    internal static class AuditDbContextConfigurationManager
    {
        internal static AuditDbContextConfiguration GetAuditDbContextConfigurationFromXml()
        {
            var auditConfigurationSection = ConfigurationManager.GetSection("entityFramework.Audit") as AuditConfigurationSection ??
                         new AuditConfigurationSection();

            var entityMapping = new List<AuditTypeInfo>();

            foreach (EntityElement entityElement in auditConfigurationSection.Entities)
            {
                var auditableEntityType = Type.GetType(entityElement.EntityType);
                if (auditableEntityType == null)
                {
                    throw new InvalidOperationException($"Auditable entity type '{entityElement.EntityType}' could not be loaded.");
                }

                var auditEntityType = Type.GetType(entityElement.AuditEntityType);
                if (auditEntityType == null)
                {
                    throw new InvalidOperationException($"Audit entity type '{entityElement.AuditEntityType}' could not be loaded.");
                }

                var auditTypeInfo = new AuditTypeInfo(auditableEntityType, auditEntityType);
                entityMapping.Add(auditTypeInfo);
            }

            return new AuditDbContextConfiguration(auditConfigurationSection.AuditEnabled, auditConfigurationSection.AuditDateTimeKind, entityMapping.ToArray());
        }
    }
}
