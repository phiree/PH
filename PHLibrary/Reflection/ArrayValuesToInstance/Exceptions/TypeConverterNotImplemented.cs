using PHLibrary.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    public class TypeConverterNotImplemented : ArrayValuesPropertyBaseException
    {
        public TypeConverterNotImplemented(Type  type,PropertyInfo property):base(property,type)
        {
           
        }
        public override string Message => $"property's type[{Property.DeclaringType.Name}]  doesn't implemented yet. please ask owner to add converter on this type in ArrayValuesToInstance.cs.{base.Message}";

    }
}
