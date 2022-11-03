using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace PHLibrary.Reflection
{
    public class ColumnMapCreator
    {
        public static IDictionary<string, string> CreateColumnMap<T>(IList<T> data)
        {
            var map = GetPropertyMaps<T>();
            if (map.Count == 0)
            {
                if (data.Count > 0)
                {
                    map = GetPropertyMapsForDynamic(data[0]);
                }


            }
            return map;
        }
        public static IDictionary<string, string> GetPropertyMaps<T>( ) //where T : class
        {
            
             
            var propertyInfos = typeof(T).GetProperties();
            var map = new Dictionary<string, string>();
            foreach (var p in propertyInfos)
            {
                var attrs = p.GetCustomAttributes(false);
                string display = p.Name;
                foreach (var attr in attrs)
                {
                    if (attr is ColumnAttribute columnAttribute)
                    {

                        display = columnAttribute == null ? p.Name : columnAttribute.Name;

                    }
               
                }
                map.Add(p.Name, display);
            }
            
            return map;

        }
        public static Tuple<string, string>   GetTwoDimensionalColumns<T>( ) //where T : class
        {
             
            string twoDimensionalColumn_X=string.Empty, twoDimensionalColumn_Y=string.Empty;
            var propertyInfos = typeof(T).GetProperties();
           
            foreach (var p in propertyInfos)
            {
                var attrs = p.GetCustomAttributes(false);
                
                foreach (var attr in attrs)
                {
                     
                    if (attr is TwoDimensionalAttribute twoDimensionalAttribute)
                    {
                        if (twoDimensionalAttribute.IsX)
                        {
                            twoDimensionalColumn_X = p.Name;
                        }
                        else
                        {
                            twoDimensionalColumn_Y = p.Name;
                        }
                    }
                }
               
            }
            if (string.IsNullOrEmpty(twoDimensionalColumn_X)||string.IsNullOrEmpty(twoDimensionalColumn_Y)) {
                return null;
            }
            return new Tuple<string, string>(twoDimensionalColumn_X, twoDimensionalColumn_Y);

          

        }
        public static IDictionary<string, string> GetPropertyMapsForDynamic(object data) //where T : class
        {
            var memberNames = Dynamitey.Dynamic.GetMemberNames(data);
            return memberNames.ToDictionary(x => x, x => x);


        }
    }
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TwoDimensionalAttribute : Attribute
    {
 
        readonly bool isX = false;//or y
 
        public TwoDimensionalAttribute(bool isX=false)
        {
            this.isX = isX;
 
        }

        public bool IsX
        {
            get { return isX; }
        }

     
    }
}
