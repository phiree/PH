using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// values' count not equal to properties'
    /// </summary>
    public class ValuesCountNotMatch  : Exception
    {
        public ValuesCountNotMatch(Type type, int propertiesCount, int valuesCount)
        {
            Type = type;
            PropertiesCount = propertiesCount;
            ValuesCount = valuesCount;
        }

        public Type Type { get; protected set; }
        public int PropertiesCount { get; protected set; }
        public int ValuesCount { get; protected set; }
        public override string Message => $"values' count not equal to properties'. type:[{nameof(Type)}].propertiesCount:[{PropertiesCount}].valuesCount:[{ValuesCount}].";


    }
}
