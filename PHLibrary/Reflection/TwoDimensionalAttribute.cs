using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TwoDimensionalAttribute : Attribute
    {

        readonly bool isX = false;//or y

        public TwoDimensionalAttribute(bool isX = false)
        {
            this.isX = isX;


        }

        public bool IsX
        {
            get { return isX; }
        }


    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class CustomAmountFormatAttribute : Attribute
    {

    }
}
