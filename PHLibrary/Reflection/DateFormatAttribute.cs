using System;

namespace PHLibrary.Reflection
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DateFormatAttribute : Attribute
    {
        public string DateFormatString { get;}
        public DateFormatAttribute(string dateFormatString) { 
            this.DateFormatString = dateFormatString;
            }
    }
}
