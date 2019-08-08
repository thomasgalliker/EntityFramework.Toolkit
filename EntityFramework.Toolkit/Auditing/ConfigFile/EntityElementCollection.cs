using System.Configuration;

namespace EntityFramework.Toolkit.Auditing.ConfigFile
{
    internal class EntityElementCollection : ConfigurationElementCollection
    {
        internal EntityElement this[int index]
        {
            get { return (EntityElement) this.BaseGet(index); }

            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new EntityElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntityElement) element).EntityType;
        }
    }
}