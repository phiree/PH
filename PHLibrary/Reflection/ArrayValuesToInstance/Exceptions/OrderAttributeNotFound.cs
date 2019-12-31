using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{

    public class OrderAttributeNotFound : Exception
    {
        public OrderAttributeNotFound(string propertyName, string typeName)
        {
            PropertyName = propertyName;
            TypeName = typeName;
        }

        public string PropertyName { get; protected set; }
        public string TypeName { get; protected set; }
        public override string Message => $"the order attribute not found on property:{TypeName}.{PropertyName}.Inner exception Message:\r\n{base.Message}";


    }
}
