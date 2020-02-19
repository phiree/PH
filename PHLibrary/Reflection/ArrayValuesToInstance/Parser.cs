using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System.Reflection;
namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// create an instance of T from an array
    /// </summary>
    /// <typeparam name="T">target type </typeparam>
    /// <typeparam name="ValueT">value type in the array</typeparam>
    public class Parser<T, ValueT> where T : new()
    {
        public T Parse(ValueT[] values)
        {
            var v = (IList<ValueT>)values;
            return Parse(v);
        }
        public T Parse(IList<ValueT> values)
        {
            var tType = typeof(T);
            var properties= tType.GetTypeInfo().DeclaredProperties;
            if(properties.Count()!=values.Count)
            { 
                throw new  ValuesCountNotMatch (tType, properties.Count(), values.Count);
                }
            T t = new T();
            int propertyOrder=0;

            foreach (var p in tType.GetTypeInfo().DeclaredProperties)
            {
                int valueOrder=propertyOrder;
                if ( p.GetCustomAttributes(false).Any(x => x.GetType() == typeof(PropertyOrderAttribute))) { 

                  valueOrder = ((PropertyOrderAttribute)(p.GetCustomAttributes(false).First(x => x.GetType() == typeof(PropertyOrderAttribute)))).Order;
                }
                
                    object value = null;
                     
                    ValueT currentValue =default;
                    try
                    { currentValue= values[valueOrder];
                    }
                    catch(ArgumentOutOfRangeException  ) {
                        throw new OrderOutOfValuesRange(p.Name,valueOrder,nameof(tType));
                        }
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
                        default: throw new TypeConverterNotImplemented(p.Name, p.PropertyType);

                    }
                try
                {
                    //p.PropertyType
                    p.SetValue(t, value);
                    propertyOrder++;
                }
                catch (Exception ex)
                {
                    throw new ValueSettingException(p.Name, valueOrder, values: string.Join("_", values), ex);
                }
            }
            return t;
        }

        public IList<T> ParseList(ValueT[,] listValues)
        {
            var result = new List<T>();
            var tType = typeof(T);
            int startIndex = 0;
            var startIndexAttr = (DataStartArrayIndexAttribute)tType.GetTypeInfo().GetCustomAttribute(typeof(DataStartArrayIndexAttribute));
            if (startIndexAttr != null)
            {
                startIndex = startIndexAttr.StartIndex;
            }
            var topLength = listValues.GetLength(0);
            var secondDimLength = listValues.GetLength(1);
            for (int i = startIndex; i < topLength; i++)
            {
                ValueT[] valueTs = new ValueT[secondDimLength];
                for (int j = 0; j < secondDimLength; j++)
                {
                    valueTs[j] = listValues[i, j];
                }
                result.Add(Parse(valueTs));


            }
            return result;
        }
    }

}
