using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System.Reflection;
using PHLibrary.Reflection.ArrayValuesToInstance.Attributes;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// create an instance of T from an array
    /// </summary>
    /// <typeparam name="T">target type </typeparam>
    /// <typeparam name="ValueT">value type in the array</typeparam>
    public class Parser<T, ValueT> where T : new()
    {
        AbsValueOrderDeterminer<T> absValueOrderDeterminer;
        IDictionary<int,PropertyInfo> propertyOrders;
        //确定数值和属性的对应关系
        Type tType;
        
        public Parser(AbsValueOrderDeterminer<T> absValueOrderDeterminer)
        {
            this.absValueOrderDeterminer=absValueOrderDeterminer;
            this.propertyOrders=absValueOrderDeterminer.DeterminePropertyOrder();
            tType = typeof(T);
         
        }
        public T Parse(ValueT[] values )
        {
            var v = (IList<ValueT>)values;
            return Parse(v );
        }

        public T Parse(IList<ValueT> values )
        {

          

            var properties = tType.GetTypeInfo().DeclaredProperties;
            T newT = new T();

            foreach (var p in tType.GetTypeInfo().DeclaredProperties)
            {
                int valueOrder = propertyOrders.First(x => x.Value.Name == p.Name).Key;

                ValueT currentValue = values[valueOrder];
                object value;
                switch (p.PropertyType.Name)
                {
                    case "String":
                        value = Convert.ToString(currentValue);
                        break;
                    case "Int16":
                        value = Convert.ToInt16(currentValue);
                        break;
                    case "Int32":
                        value = Convert.ToInt32(currentValue);
                        break;
                    case "DateTime":
                        value = Convert.ToDateTime(currentValue);
                        break;
                    case "Decimal":
                        value = Convert.ToDecimal(currentValue);
                        break;
                    default: throw new TypeConverterNotImplemented(typeof(T), p);

                }
                try
                {
                    //p.PropertyType
                    p.SetValue(newT, value);

                }
                catch (Exception ex)
                {
                    throw new ValueSettingException(typeof(T), p, valueOrder, values: string.Join("_", values));
                }
            }
            return newT;
        }

        public IList<T> ParseList(ValueT[,] listValues)
        {
            
            var result = new List<T>();
            var topLength = listValues.GetLength(0);
            var secondDimLength = listValues.GetLength(1);
            int startIndex = 0;
            if (absValueOrderDeterminer.GetType() == typeof(FirstArrayDeterminer<T>))
            {
                startIndex = 1;
            }
            for (int i = startIndex; i < topLength; i++)
            {
                ValueT[] valueTs = new ValueT[secondDimLength];
                for (int j = 0; j < secondDimLength; j++)
                {
                    valueTs[j] = listValues[i, j];
                }
                result.Add(Parse(valueTs ));


            }
            return result;
        }
    }

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
