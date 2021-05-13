using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PHLibrary.Reflection
{
   public class ColumnMapCreator
    {
        public static IDictionary<string, string> CreateColumnMap<T>(IList<T>  data)
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
        public  static IDictionary<string, string> GetPropertyMaps<T>() //where T : class
        {
            var propertyInfos = typeof(T).GetProperties();
            var map = new Dictionary<string, string>();
            foreach (var p in propertyInfos)
            {
                var attr = p.GetAttribute<DisplayNameAttribute>(false);
                string display = attr == null ? p.Name : attr.DisplayName;
                map.Add(p.Name, display);
            }
            return map;

        }
        public static  IDictionary<string, string> GetPropertyMapsForDynamic(object data) //where T : class
        {
            var memberNames = Dynamitey.Dynamic.GetMemberNames(data);
            return memberNames.ToDictionary(x => x, x => x);


        }
    }
}
