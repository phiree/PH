using System.Collections.Generic;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System.Reflection;
using System.Linq;
namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// property - value array order match.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbsValueOrderDeterminer<T>
    {
        protected IDictionary<int, PropertyInfo> PropertyOrder = new Dictionary<int, PropertyInfo>();
        public IEnumerable<PropertyInfo> Properties { get; private set; }
        protected string[] FirstArray { get;private set;}
        protected AbsValueOrderDeterminer(string[] firstArray)
        {
            Properties = typeof(T).GetTypeInfo().DeclaredProperties;
            FirstArray=firstArray;
            if(firstArray.Length!=Properties.Count())
            { 
                throw new ValuesCountNotMatch(typeof(T).Name,firstArray.Length);
                }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>key-value pares of propertyname and order in value list.</returns>
        public abstract IDictionary<int, PropertyInfo> DeterminePropertyOrder();
    }

}
