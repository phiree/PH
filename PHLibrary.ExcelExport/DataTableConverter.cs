using Dynamitey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PHLibrary.Reflection.DisplayAttribute;
using System.Data.Common;
using PHLibrary.Reflection.ArrayValuesToInstance;
using System.Collections.Specialized;

namespace PHLibrary.ExcelExportExcelCreator
{
    /// <summary> 
    /// convert T to datatable
    /// </summary>
    public class DataTableConverter<T> //where T : class
    {

        private IDictionary<string, string> GetPropertyMaps<T>() //where T : class
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

        public DataTable Convert(IList<T> data, IDictionary<string, string> propertyNameMaps = null)
        {
            if (propertyNameMaps == null)
            {

                propertyNameMaps = PHLibrary.Reflection.DisplayAttribute.ReflectHelper.GetPropertyMaps<T>();
            }
            if (data.Count == 0)
            {
                throw new Exception("没有数据,不能导出");
            }
            var firstData = data[0];
            var memberNames = Dynamitey.Dynamic.GetMemberNames(firstData);
            var dataTable = new DataTable("Sheet1");
            var unOrderedColumns = new Dictionary<DataColumn, int>();
            for (int i = 0; i < memberNames.Count(); i++)// var name in memberNames)
            {
                int orderNo = i + 1;
                string name = memberNames.ElementAt(i);
                string columnName = name;

                try
                {

                    columnName = propertyNameMaps[name];
                }
                catch
                {
                    throw new PropertyMapMatchNotFound(name);
                }

                //var order = typeof(T).GetProperty(name).GetAttribute<PropertyOrderAttribute>(false);
                //{
                //    if (order != null)
                //    {
                //        orderNo = order.Order;
                //    }
                //}


                var column = new DataColumn(columnName);
                column.Caption = name;
                unOrderedColumns.Add(column, orderNo);

            }
            foreach (var column in unOrderedColumns.OrderBy(x => x.Value))
            {
                dataTable.Columns.Add(column.Key);
            }
            Debug.Assert(memberNames.Count() == dataTable.Columns.Count, "数据列和属性数量应该相等");
            foreach (T t in data)
            {
                var row = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    string propertyName = column.Caption;
                    var value = Dynamic.InvokeGet(t, propertyName);

                    row[column.ColumnName] = value;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

    }
}
