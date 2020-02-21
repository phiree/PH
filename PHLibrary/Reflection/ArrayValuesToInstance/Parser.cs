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
                try{
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
                }
                catch(Exception ex)
                { 
                    throw new ConvertFailure<ValueT>(typeof(T),p,currentValue);
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

}
