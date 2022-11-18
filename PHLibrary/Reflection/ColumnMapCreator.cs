using PHLibrary.ExcelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection
{
    public class ColumnMapCreator
    {

        public IList<ColumnDefine> CreateColumnMap<T>(IList<string> propertiesToDisplay)
        {
            var map = GetPropertyMaps<T>(propertiesToDisplay);

            return map;
        }
        public class ColumnDefine
        {
            public ColumnDefine(string propertyName, string displayName, object[] attributes)
            {
                PropertyName = propertyName;
                DisplayName = displayName;
                Attributes = attributes;
            }

            public string PropertyName { get; set; }
            public string DisplayName { get; set; }
            public object[] Attributes { get; set; } = new object[] { };


        }
        public IList<ColumnDefine> GetPropertyMaps<T>(IList<string> propertiesToDisplay) //where T : class
        {


            var propertyInfos = typeof(T).GetProperties();
            var map = new List<ColumnDefine>();

             

            foreach (var p in propertyInfos)
            {


                var attrs = p.GetCustomAttributes(false);
                //哪些列需要显示
                // 二维列即便没有出现在列表中，也依旧能显示
                if (!attrs.Any(x => x.GetType() == typeof(TwoDimensionalAttribute) || x.GetType() == typeof(TwoDimensionalGuidAttribute))
                    && !propertiesToDisplay.Any(x => x.ToLower() == p.Name.ToLower()
                    ))

                {
                    continue;
                }
                string display = GetDisplayName(p, attrs);
                map.Add(new ColumnDefine(p.Name, display, attrs));
                
            }
           return  map.OrderBy(x=>propertiesToDisplay.Select(s=>s.ToLower()).ToList().IndexOf(x.PropertyName.ToLower())).ToList();
            return map;

        }

        private static string GetDisplayName(PropertyInfo p, object[] attrs)
        {
            string display = p.Name;
            foreach (var attr in attrs)
            {

                //只有设置了propertyorder 的属性才需要导出

                if (attr is ColumnAttribute columnAttribute)
                {
                    display = columnAttribute == null ? p.Name : columnAttribute.Name;
                }


            }

            return display;
        }

        public static IList<ColumnDefine> GetPropertyMapsForDynamic(object data) //where T : class
        {
            var memberNames = Dynamitey.Dynamic.GetMemberNames(data);
            return memberNames.Select(x => new ColumnDefine(x, x, null)).ToList();


        }
    }


}
