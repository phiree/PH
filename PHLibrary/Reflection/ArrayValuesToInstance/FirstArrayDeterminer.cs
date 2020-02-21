using System;
using System.Collections.Generic;
using System.Linq;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System.Reflection;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// match with first array in value list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FirstArrayDeterminer<T> : AbsValueOrderDeterminer<T>
    {
      
        public FirstArrayDeterminer(string[] firstArray) : base(firstArray)
        {
           
        }
        public override IDictionary<int, PropertyInfo> DeterminePropertyOrder()
        {
             

            for (int i = 0; i < FirstArray.Length; i++)
            {
                try
                {
                    var p = Properties.First(x => x.Name.ToLower() == FirstArray[i].ToLower());
                    PropertyOrder.Add(i, p);
                }
                catch (InvalidOperationException)
                {
                    throw new PropertyNotFound(FirstArray[i], typeof(T));
                }
            }
            return PropertyOrder;
        }
    }

}
