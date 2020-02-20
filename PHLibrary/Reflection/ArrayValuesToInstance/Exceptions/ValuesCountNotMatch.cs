using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
namespace PHLibrary.Reflection.ArrayValuesToInstance.Exceptions
{
    /// <summary>
    /// values' count not equal to properties'
    /// </summary>
    public class ValuesCountNotMatch  :Exception
    {
        
        public ValuesCountNotMatch(string   type,   int valuesCount) 
        {
             
            ValuesCount = valuesCount;
            Type=type;
        }

       
       public string Type { get;protected set;}
        public int ValuesCount { get; protected set; }
        public override string Message => $"values' count [{ValuesCount}] not equal to properties'[ {Type}]. {base.Message}";


    }
}
