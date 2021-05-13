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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PHLibrary.ExcelExportExcelCreator
{
    /// <summary> 
    /// convert T to datatable
    /// </summary>
    public class DataTableConverter<T> : IDataTableConverter<T>
    //where T : class
    {
        ILogger logger;
        public DataTableConverter(ILoggerFactory loggerFactory = null)
        {
            if (loggerFactory == null) { logger = NullLogger.Instance; }
            else
            {
                logger = loggerFactory.CreateLogger<DataTableConverter<T>>();
            }
        }


        public DataTable Convert(IList<T> data, IDictionary<string, string> propertyNameMaps = null)
        {

            if (propertyNameMaps == null)
            {

                propertyNameMaps = ReflectHelper.GetPropertyMaps<T>();
            }
            IEnumerable<string> memberNames;
            // no dynamics
            memberNames = typeof(T).GetProperties().Select(x => x.Name);
            if (memberNames == null || memberNames.Count() == 0)
            {
                if (data.Count == 0)
                {
                    string errMsg = "数据类型为 dynamic, 且没有数据,无法推断出列名,无法导出";
                    logger.LogError(errMsg);
                    throw new Exception(errMsg);
                }
                var firstData = data[0];
              memberNames = Dynamitey.Dynamic.GetMemberNames(firstData);
            }
            if (memberNames == null || memberNames.Count() == 0)
            {
                string errMsg = $"无法计算列名信息,无法导出.T:{typeof(T).Name}";
                logger.LogError(errMsg);
                throw new Exception(errMsg);
            }




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
                try
                {
                    var orderProperty = typeof(T).GetProperty(name);
                    if (orderProperty == null)
                    {
                        logger.LogWarning("OrderProperty is null");
                    }
                    else
                    {
                        var order = orderProperty.GetAttribute<PropertyOrderAttribute>(false);
                        {
                            if (order != null)
                            {
                                orderNo = order.Order;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Get OrderPropertyError :" + ex.ToString());
                }


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
