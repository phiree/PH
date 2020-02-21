using System.Collections.Generic;
using System.Linq;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System.Reflection;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// match using property attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyAttributeDeterminer<T> : AbsValueOrderDeterminer<T>
    {
        public PropertyAttributeDeterminer(string[] firstArray):base(firstArray)
        { }
        public override IDictionary<int, PropertyInfo> DeterminePropertyOrder()
        {
            int order = 0;
            IList<bool> orderAttributes = new List<bool>();
          
            foreach (var p in Properties)
            {

                if (p.GetCustomAttributes(false).Any(x => x.GetType() == typeof(PropertyOrderAttribute)))
                {
                    orderAttributes.Add(true);
                    order = ((PropertyOrderAttribute)(p.GetCustomAttributes(false).First(x => x.GetType() == typeof(PropertyOrderAttribute)))).Order;
                    if(order>=Properties.Count())
                    { 
                        throw new OrderOutOfValuesRange(p,typeof(T),order);
                        }
                  
                   
                    }
                else
                {
                    orderAttributes.Add(false);
                   
                }
                if (orderAttributes.Distinct().Count() > 1)
                {
                    throw new PartlyOrderAttribute(typeof(T));
                }

                PropertyOrder.Add(order, p);
                order++;

            }
            return PropertyOrder;
        }
    }

}
