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
using System.Xml.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

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

                var columnType=new ColumnDataTypeDetermine<T>().GetPropertyType(data,columnName);
               //guess column type using first row of data
               var column = new DataColumn(columnName,columnType);
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

    //列的数据类型定义
    /// <summary>
    /// t 有可能是匿名类。
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColumnDataTypeDetermine<T> { 
        
         public Type GetPropertyType(IList<T> data,string propertyName){

         var property=    typeof(T).GetProperty(propertyName);
            if(property==null){
                //匿名类
                if (data.Count > 0) {
var firstData = data[0];
                 var propertyValue=   Dynamic.InvokeGet(firstData, propertyName);
                    return propertyValue.GetType();
                }
                }
            else { 
                return property.GetType();
                }
            return typeof(string);
        }
        }
}
