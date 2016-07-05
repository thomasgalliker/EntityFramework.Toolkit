namespace EntityFramework.Toolkit
{
    public class PropertyChangeInfo
    {
        public PropertyChangeInfo(string propertyName, object currentValue)
        {
            this.PropertyName = propertyName;
            this.CurrentValue = currentValue;
        }

        public object CurrentValue { get; internal set; }

        public string PropertyName { get; internal set; }
    }
}