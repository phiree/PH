using PHLibrary.ExcelExport;
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
        bool needExportImage;

        public ColumnMapCreator(bool needExportImage)
        {
            this.needExportImage = needExportImage;
        }
        public IList< ColumnDefine> CreateColumnMap<T>(IList<T> data)
        {
            var map = GetPropertyMaps<T>();
            //if (map.Count == 0)
            //{
            //    if (data.Count > 0)
            //    {
            //        map = GetPropertyMapsForDynamic(data[0]);
            //    }


            //}
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
            public object[] Attributes { get; set; }=new object[] { };


        }
        public IList< ColumnDefine> GetPropertyMaps<T>() //where T : class
        {


            var propertyInfos = typeof(T).GetProperties();
            var map = new List<ColumnDefine>();
            foreach (var p in propertyInfos)
            {
                var attrs = p.GetCustomAttributes(false);
                string display = p.Name;
                bool shouldIgnore = true;
                foreach (var attr in attrs)
                {
                    if (attr is ImageColumnAttribute && !needExportImage)
                    {
                        shouldIgnore = true;
                        break;
                    }
                    //只有设置了propertyorder 的属性才需要导出

                    if (attr is ColumnAttribute columnAttribute)
                    {
                        display = columnAttribute == null ? p.Name : columnAttribute.Name;
                    }
                    else if (attr is PHLibrary.Reflection.ArrayValuesToInstance.PropertyOrderAttribute
                        || attr is TwoDimensionalGuidAttribute
                        || attr is TwoDimensionalAttribute
                        )
                    {
                        shouldIgnore = shouldIgnore && false;

                    }

                }
                if (!shouldIgnore)
                {
                    map.Add( new ColumnDefine(p.Name,display,attrs));
                }
            }

            return map;

        }


        public static IList<ColumnDefine> GetPropertyMapsForDynamic(object data) //where T : class
        {
            var memberNames = Dynamitey.Dynamic.GetMemberNames(data);
            return memberNames.Select(x =>  new ColumnDefine(x,x,null)).ToList();


        }
    }


}
