using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// orderno out of values range.
    /// </summary>
    public class OrderOutOfValuesRange : ArrayValuesPropertyBaseException
    {
        public OrderOutOfValuesRange(PropertyInfo property, Type type, int propertyOrder) : base(property, type)
        {
            
            PropertyOrder = propertyOrder;
            
        }
 
        public int PropertyOrder { get;protected set;}
        public override string Message => $"orderno [{ PropertyOrder}] out of values range.{base.Message}";


    }
}
