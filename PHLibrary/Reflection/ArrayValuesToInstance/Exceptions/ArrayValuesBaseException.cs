using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// exception on Typeinfo
    /// </summary>
   public class ArrayValuesTypeBaseException:Exception
    {
        public ArrayValuesTypeBaseException(Type type):this(type,null)
        {
           
        }

        public ArrayValuesTypeBaseException(Type type,Exception innerException):base(string.Empty,innerException) 
        {
            Type = type;
        }

        public Type Type { get;protected set;}
        public override string Message => $"Type:[{Type.Name}].InnerException:[{InnerException}]";
    }
    /// <summary>
    /// exceptions on Type and Property
    /// </summary>
    public class ArrayValuesPropertyBaseException : ArrayValuesTypeBaseException
    {
        public ArrayValuesPropertyBaseException(PropertyInfo property,Type type):base(type)
        {
            Property = property;
        }

        public PropertyInfo Property { get; protected set; }
        public override string Message => $"{base.Message},Property:[{Property.Name}]";
    }
}
