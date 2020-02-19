using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// orderno out of values range.
    /// </summary>
    public class OrderOutOfValuesRange : Exception
    {
        public OrderOutOfValuesRange(string propertyName,int propertyOrder, string typeName)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            this.PropertyName=propertyName;
        }

        public string PropertyName { get; protected set; }
        public string TypeName { get; protected set; }
        public int PropertyOrder { get;protected set;}
        public override string Message => $"orderno out of values range.type:{TypeName}.{PropertyName}.propertyOrder:{PropertyOrder}";


    }
}
