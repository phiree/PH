using PHLibrary.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    public class TypeConverterNotImplemented : PHBaseException
    {
        public TypeConverterNotImplemented(string propertyName, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;

        }

        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }

        public override string Message => $"property[{PropertyName}]'s type[{PropertyType}]  doesn't implemented yet. please ask owner to add converter on this type in ArrayValuesToInstance.cs.{base.Message}";

    }
}
