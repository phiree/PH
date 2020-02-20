using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// order attribute should be assign to all properties or none, can't be partly.
    /// </summary>
    public class PartlyOrderAttribute:ArrayValuesTypeBaseException
    {
        public PartlyOrderAttribute(Type  type):base(type)
        {
            
        }

       
        public override string Message => $"order attribute should be assign to all properties or none, can't be partly assigned. {base.Message}";
    }
}
