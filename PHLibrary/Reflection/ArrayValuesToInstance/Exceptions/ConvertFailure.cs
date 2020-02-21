using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    public class ConvertFailure<T>:ArrayValuesPropertyBaseException
    {
        public ConvertFailure(Type type,PropertyInfo property,T value) : base(property, type)
        {this.Value=value;
        }
        public T Value { get;protected set;}
        public override string Message =>$"raw value:[{typeof(T)}:{Value}],{base.Message}";


    }
}
