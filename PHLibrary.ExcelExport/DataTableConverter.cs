using Dynamitey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PHLibrary.Reflection;
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

                propertyNameMaps = ColumnMapCreator.CreateColumnMap<T>(data);
            }
            
                var dataTable = new DataTable("Sheet1");
            var unOrderedColumns = new Dictionary<DataColumn, int>();
            for (int i = 0; i < propertyNameMaps.Count(); i++)// var name in memberNames)
            {
                int orderNo = i + 1;
                string name = propertyNameMaps.ElementAt(i).Key;
                string columnName = propertyNameMaps.ElementAt(i).Value;
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
