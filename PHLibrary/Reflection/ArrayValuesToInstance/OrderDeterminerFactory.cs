using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    public class OrderDeterminerFactory<T>
    {
        string[] firstArray;
        public OrderDeterminerFactory(string[] firstArray)
        {
            this.firstArray = firstArray;
        }
        public AbsValueOrderDeterminer<T> Create(EnumPropertyOrderDeterminerType enumPropertyOrderDeterminerType)
        {
            switch (enumPropertyOrderDeterminerType)
            {
                case EnumPropertyOrderDeterminerType.FirstArray:
                    return new FirstArrayDeterminer<T>(firstArray);

                case EnumPropertyOrderDeterminerType.OrderAttribute:
                    return new PropertyAttributeDeterminer<T>(firstArray);
                    ;
                default: throw new NotImplementedException();
            }
        }
    }
    public enum EnumPropertyOrderDeterminerType
    {
        FirstArray,
        OrderAttribute
    }
}
