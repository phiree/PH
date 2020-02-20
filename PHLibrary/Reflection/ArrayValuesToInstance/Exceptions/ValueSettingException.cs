using PHLibrary.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{

    public class ValueSettingException : ArrayValuesPropertyBaseException
    {
        public ValueSettingException(Type  type,PropertyInfo property, int order, string values ) : base(property,type)
        {
            
            Order = order;
            Values = values;
        }

        public string PropertyName { get; set; }
        public int Order { get; set; }
        public string Values { get; set; }

        public override string Message
            => $"Error occurred when setting value on property:[{PropertyName}] in order:[{Order}]. Values:{string.Join("_", Values)}.{base.Message}";
    }
}
