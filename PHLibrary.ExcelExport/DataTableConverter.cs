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
using System.Drawing;
using System.Net.Http;
using System.IO;

namespace PHLibrary.ExcelExportExcelCreator
{

    public class DataTableWithExtensions
    {
        public DataTable DataTable { get; set; }
        public Dictionary<int, Image> Images { get; set; }

    }

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
                        var attributes = orderProperty.GetCustomAttributes(false);

                        foreach (var attribute in attributes)
                        {
                            if (attribute is PropertyOrderAttribute)
                            {
                                orderNo = ((PropertyOrderAttribute)attribute).Order;
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Get OrderPropertyError :" + ex.ToString());
                }

                var columnType = new ColumnDataTypeDetermine<T>().GetPropertyType(data, name);
                //guess column type using first row of data
                var column = new DataColumn(columnName, columnType);
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

                    if (column.DataType == typeof(System.Drawing.Image))
                    {
                        row[column.ColumnName] = DownloadImageAsync(value);
                    }
                    else
                    {

                        row[column.ColumnName] = value;
                    }

                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        private Image DownloadImageAsync(string uri)
        {
            var httpClient = new HttpClient();


            // Download the image and write to the file
            var imageBytes = httpClient.GetByteArrayAsync(uri).Result;
           var ms = new MemoryStream(imageBytes) ;
             
               
                
                var image=new Bitmap( Image.FromStream(ms));
         
                image.Save("d:\\a.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                return image;
           

        }
    }


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
                var imageAttribute = property.GetAttribute<ImageColumnAttribute>(false);
                if (imageAttribute != null)
                {
                    return typeof(System.Drawing.Image);
                }

                return property.PropertyType;
            }
            return typeof(string);
        }
    }
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ImageColumnAttribute : Attribute
    {
    }
}
