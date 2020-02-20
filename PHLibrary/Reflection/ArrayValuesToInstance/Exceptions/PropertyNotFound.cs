using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    public class PropertyNotFound:ArrayValuesTypeBaseException
    {
        public PropertyNotFound(string propertyName, Type  type) : base(type)
        {
            PropertyName = propertyName;
        }
        public string PropertyName { get;protected set;}
    }
}
