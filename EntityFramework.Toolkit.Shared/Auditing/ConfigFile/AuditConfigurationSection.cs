using System.Configuration;

namespace EntityFramework.Toolkit.Auditing.ConfigFile
{
    internal class AuditConfigurationSection : ConfigurationSection
    {
        private const string EntitiesElementName = "entities";
        private const string AuditEnabledElementName = "enabled";

        [ConfigurationProperty(EntitiesElementName, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EntityElementCollection), AddItemName = "add")]
        internal EntityElementCollection Entities
        {
            get
            {
                return (EntityElementCollection)base[EntitiesElementName];
            }
        }

        /// <summary>
        ///     Turns auditing on (=<code>true</code>) or off (=<code>false</code>). Default=<code>true</code>.
        /// </summary>
        [ConfigurationProperty(AuditEnabledElementName, IsRequired = false, DefaultValue = true)]
        internal bool AuditEnabled
        {
            get
            {
                return (bool)this[AuditEnabledElementName];
            }

            set
            {
                this[AuditEnabledElementName] = value;
            }
        }
    }
}