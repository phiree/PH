using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance
{
    /// <summary>
    /// instance's start index in array[] .
    /// </summary>
    public class DataStartArrayIndexAttribute : Attribute
    {
        
        public DataStartArrayIndexAttribute(int startIndex)
        {
            this.StartIndex = startIndex;
        }
        public int StartIndex { get; set; }
    }
}
