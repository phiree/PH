using Dynamitey;
using System;
using System.Collections.Generic;
using PHLibrary.Reflection;

namespace PHLibrary.ExcelExportExcelCreator
{
    //列的数据类型定义
    /// <summary>
    /// t 有可能是匿名类。
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColumnDataTypeDetermine<T>
    {

        public Type GetPropertyType(IList<T> data, string propertyName)
        {

            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
            {
                //匿名类
                if (data.Count > 0)
                {
                    var firstData = data[0];
                    var propertyValue = Dynamic.InvokeGet(firstData, propertyName);
                    return propertyValue.GetType();
                }
            }
            else
            {
                 
                var attributes = property.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    if (attr is ImageColumnAttribute)
                    {
                        return typeof(System.Drawing.Image);
                    }
                    else if (attr is CustomAmountFormatAttribute)
                    {
                        return typeof(double);
                    }
                }


                return property.PropertyType;
            }
            return typeof(string);
        }
    }
}
