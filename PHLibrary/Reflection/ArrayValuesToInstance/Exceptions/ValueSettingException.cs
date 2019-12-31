using PHLibrary.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{

    public class ValueSettingException : PHBaseException
    {
        public ValueSettingException(string propertyName, int order, string values, Exception baseException) : base(baseException)
        {
            PropertyName = propertyName;
            Order = order;
            Values = values;
        }

        public string PropertyName { get; set; }
        public int Order { get; set; }
        public string Values { get; set; }

        public override string Message
            => $"Error occurred when setting value.property:{PropertyName},order:{Order}，values:{string.Join("_", Values)}.Inner message:{BaseException.ToString()}";
    }
}
