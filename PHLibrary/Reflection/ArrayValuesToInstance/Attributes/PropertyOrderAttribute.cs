using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    public class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// corresponding order of the property in array values.Begin with 0;
        /// </summary>
        /// <param name="order">order</param>
        public PropertyOrderAttribute(int order)
        {
            this.Order = order;
        }
        public int Order { get; set; }
    }
}
