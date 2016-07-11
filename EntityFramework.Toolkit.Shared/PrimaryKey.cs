using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EntityFramework.Toolkit
{
    public class PrimaryKey
    {
        public PrimaryKey(PropertyInfo propertyInfo, object value)
        {
            this.PropertyInfo = propertyInfo;
            this.Value = value;
        }

        public object Value { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }
    }
}
