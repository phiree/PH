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
using System.Dynamic;
using System.Collections;
using PHLibrary.ExcelExport;

namespace PHLibrary.ExcelExportExcelCreator
{

    public class DataTableWithExtensions
    {
        public DataTable DataTable { get; set; }
        public Dictionary<int, Image> Images { get; set; }

    }
    public class TwoDimensionalDefine
    {
        public TwoDimensionalX TwoDimensionalX { get; set; }
       
        public string YName { get; set; }

        public TwoDimensionalDefine(string xName, string xGuid, string yName)
        {
            TwoDimensionalX=new TwoDimensionalX(xName,xGuid);
            
            YName = yName;
        }
        public bool Contains(string columnName)
        {
            return TwoDimensionalX.Name == columnName || TwoDimensionalX.Guid == columnName || YName == columnName;
        }
    }
    public class TwoDimensionalX
    { 
        public string Name;
        public string Guid;

        public TwoDimensionalX(string name, string guid)
        {
            Name = name;
            Guid = guid;
        }
    }
    public delegate IList<string> SortSize(IList<TwoDimensionalX> x);
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



        public class ColumnsBuilder
        {

            /// <summary>
            /// 包含二维数据的列
            /// </summary>
            public IList<DataColumn> NotTwoDimensinalColumns { get; private set; } = new List<DataColumn>();
            /// <summary>
            /// 不包含二维数据的列
            /// </summary>
            public IList<DataColumn> TwoDimensinalColumns { get; private set; } = new List<DataColumn>();
            public void Build(DataTable originalTable, TwoDimensionalDefine twoDimentinalColumnNames)
            {


                foreach (DataColumn column in originalTable.Columns)
                {

                    if ( twoDimentinalColumnNames.Contains(column.ColumnName))
                    {
                        TwoDimensinalColumns.Add(column);
                    }
                    else
                    {
                        NotTwoDimensinalColumns.Add(column);
                    }



                }
            }

            

        }

        public DataTable ConvertToTwoDimentioanl(DataTable originalTable, TwoDimensionalDefine twoDimentinalColumnNames, SortSize sortSize)
        {

            ColumnsBuilder columnsBuilder = new ColumnsBuilder();
            columnsBuilder.Build(originalTable, twoDimentinalColumnNames);

            

            //var twoDimentionalColumnValues = originalTable.AsEnumerable().Select(x => x[twoDimentinalColumnNames.Item1].ToString()).Distinct();
            //foreach (var twoDementinalColumnValue in twoDimentionalColumnValues)
            //{

            //    newColumns.Add(new DataColumn(twoDementinalColumnValue, twoDementinalColumnValue.GetType()));
            //}

            var columnsGroupByNoTwoDimensinal = originalTable.AsEnumerable()
                         .GroupBy(x => GetGroupBy(columnsBuilder.NotTwoDimensinalColumns.Select(c => c.ColumnName), x))
                         //.GroupBy(x=>new{品名= x.Field<string>("品名"),颜色=x.Field<string>("颜色") })
                         .Select((g, v) =>
                         {
                             return g;
                         }).ToList();

            IList<TwoDimensionalX> newColumns2 = columnsGroupByNoTwoDimensinal.SelectMany(x => x.ToList())
                .Select(x =>new TwoDimensionalX(
                    x[twoDimentinalColumnNames.TwoDimensionalX.Name].ToString(),
                    x[twoDimentinalColumnNames.TwoDimensionalX.Guid]==null ?"": x[twoDimentinalColumnNames.TwoDimensionalX.Guid].ToString())
                    )
                .Distinct()
                .ToList();

           
            IList<string> orderedTwoDimensionalColumns= sortSize(newColumns2);
            var allColumns = new List<DataColumn>();

            foreach (var col in columnsBuilder.NotTwoDimensinalColumns)
            {
                allColumns.Add(col); 
            }
            foreach (var col2 in orderedTwoDimensionalColumns)
            {
                allColumns.Add(new DataColumn(col2.ToString()));
            }
            

            DataTable newt = new DataTable();

            foreach (var col in allColumns)
            {
                newt.Columns.Add(new DataColumn(col.ColumnName, col.DataType)); ;
            }


            foreach (var group in columnsGroupByNoTwoDimensinal)
            {
                var row = newt.NewRow();
                foreach (DataColumn col in newt.Columns)
                {
                    if (((IDictionary<string, object>)group.Key).ContainsKey(col.ColumnName))
                    {
                        row[col.ColumnName] = ((IDictionary<string, object>)group.Key)[col.ColumnName];
                    }
                    foreach (var r in group.ToList())
                    {
                        if (r[twoDimentinalColumnNames.TwoDimensionalX.Name].ToString() == col.ColumnName)
                        {
                            row[col.ColumnName] = r[twoDimentinalColumnNames.YName];

                        }
                    }
                }
                newt.Rows.Add(row);
            }
            return newt;

        }

        IList<ExpandoObject> keyObjects = new List<ExpandoObject>();

        private ExpandoObject GetGroupBy(IEnumerable<string> columns, DataRow row)
        {

            var dict = new Dictionary<string, object>();
            foreach (string c in columns)
            {

                dict[c] = row[c];
            }

            var eo = dict.ToExpando();
            foreach (var keyObject in keyObjects)
            {
                if (AreExpandosEquals(keyObject, eo))
                    return keyObject;
            }
            keyObjects.Add(eo);
            return eo;

        }
        public static TwoDimensionalDefine GetTwoDimensionalColumns<T>() //where T : class
        {

            string twoDimensionalColumn_X = string.Empty, twoDimensionalColumn_Y = string.Empty;
            string twoDimensionalColumn_XId = string.Empty;
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
                    if (attr is TwoDimensionalGuidAttribute twoDimensionalGuidAttribute)
                    {
                        twoDimensionalColumn_XId = p.Name;
                    }
                }

            }
            if (string.IsNullOrEmpty(twoDimensionalColumn_X) || string.IsNullOrEmpty(twoDimensionalColumn_Y))
            {
                return null;
            }
            return new TwoDimensionalDefine(twoDimensionalColumn_X, twoDimensionalColumn_XId, twoDimensionalColumn_Y);



        }
        public static bool AreExpandosEquals(ExpandoObject obj1, ExpandoObject obj2)
        {
            var obj1AsColl = (ICollection<KeyValuePair<string, object>>)obj1;
            var obj2AsDict = (IDictionary<string, object>)obj2;

            // Make sure they have the same number of properties
            if (obj1AsColl.Count != obj2AsDict.Count)
                return false;

            foreach (var pair in obj1AsColl)
            {
                // Try to get the same-named property from obj2
                object o;
                if (!obj2AsDict.TryGetValue(pair.Key, out o))
                    return false;

                // Property names match, what about the values they store?
                if (!object.Equals(o, pair.Value))
                    return false;
            }

            // Everything matches
            return true;
        }
        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// </summary>
        public DataTable Convert(IList<T> data, SortSize sortSize)
        {


            var propertyNameMaps = ColumnMapCreator.CreateColumnMap<T>(data);

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
                var finalColumns = new List<DataColumn>();
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

            var twoDimensionalColumns = GetTwoDimensionalColumns<T>();
            if (twoDimensionalColumns != null)
            {
                if (sortSize == null)
                {
                    throw new Exception("必须提供二维列排序委托");
                }
                dataTable = ConvertToTwoDimentioanl(dataTable, twoDimensionalColumns, sortSize);
            }

            return dataTable;
        }
        Dictionary<string, Image> imageDict = new Dictionary<string, Image>();
        private Image DownloadImageAsync(string uri)
        {
            if (imageDict.ContainsKey(uri))
            {
                return imageDict[uri];
            }
            var httpClient = new HttpClient();


            // Download the image and write to the file
            var imageBytes = httpClient.GetByteArrayAsync(uri).Result;
            var ms = new MemoryStream(imageBytes);



            var image = new Bitmap(Image.FromStream(ms));

            image.Save("d:\\a.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            imageDict.Add(uri, image);
            return image;


        }
    }

    public static class DictToExpando
    {
        /// <summary>
        /// Extension method that turns a dictionary to an ExpandoObject, recursively (sub-dictionaries and sub-collections are also turned into ExpandoObjects).
        /// If TKey is not the String type and no keyTransformer has been provided, an InvalidCastException will be thrown (unless the dictionary is empty). For
        /// sub-dictionaries, if an Exception occurs during the transformation of dictionary to ExpandoObject, the sub-dictionary is left as is and not transformed.
        /// </summary>
        /// <param name="dictionary">The dictionary to transform into an ExpandoObject</param>
        /// <param name="keyTransformer">An optional delegate function who will be passed each dictionary key and must return the corresponding string key</param>
        /// <returns>The ExpandoObject</returns>
        /// <throws>InvalidCastException when a non-string key has been found by the default keyTransformer</throws>
        public static ExpandoObject ToExpando<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<object, string> keyTransformer = null)
        {
            if (keyTransformer == null)
            {
                // When no keyTransformer has been provided, simply assume every key is a string (will throw InvalidCastException when not the case)
                keyTransformer = delegate (object key) { return (string)key; };
            }
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                if (kvp.Value is System.Collections.IDictionary || (kvp.Value != null && kvp.Value.GetType().IsGenericType && kvp.Value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    // if value is a dictionary (generic or non-generic), convert it to ExpandoObject
                    expandoDic.Add(keyTransformer(kvp.Key), TryConvertToExpandoIfDictionary(kvp.Value));
                }
                else if (kvp.Value != null && null != kvp.Value.GetType().GetInterface("System.Collections.ICollection"))
                {
                    // if value is a collection, convert it to ExpandoObject
                    expandoDic.Add(keyTransformer(kvp.Key), TryConvertToExpandoIfCollection(kvp.Value));
                }
                else
                {
                    expandoDic.Add(keyTransformer(kvp.Key), kvp.Value);
                }
            }
            return expando;
        }

        /// <summary>
        /// Extension method that turns a dictionary to an ExpandoObject, recursively (sub-dictionaries and sub-collections are also turned into ExpandoObjects).
        /// When a non-string key is found and no keyTransformer has been provided, an InvalidCastException will be thrown. For sub-dictionaries, if an Exception
        /// occurs during the transformation of dictionary to ExpandoObject, the sub-dictionary is left as is and not transformed.
        /// </summary>
        /// <param name="dictionary">The dictionary to transform into an ExpandoObject</param>
        /// <param name="keyTransformer">An optional delegate function who will be passed each dictionary key and must return the corresponding string key</param>
        /// <returns>The ExpandoObject</returns>
        /// <throws>InvalidCastException when a non-string key has been found by the default keyTransformer</throws>

        private static object TryConvertToExpandoIfDictionary(dynamic dictionary, Func<object, string> keyTransformer = null)
        {
            if (keyTransformer == null)
            {
                keyTransformer = delegate (object key) { return (string)key; };
            }
            try
            {
                if (dictionary is System.Collections.IDictionary || (dictionary.GetType().IsGenericType && dictionary.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    return dictionary.ToExpando(keyTransformer);
                }
            }
            catch (Exception) { }
            return dictionary;
        }

        private static object TryConvertToExpandoIfCollection(object collection, Func<object, string> keyTransformer = null)
        {
            try
            {
                if (null != collection.GetType().GetInterface("System.Collections.ICollection"))
                {
                    var newList = new List<object>();
                    foreach (var item in (System.Collections.ICollection)collection)
                    {
                        if (item is System.Collections.IDictionary || (item != null && item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                        {
                            newList.Add(TryConvertToExpandoIfDictionary(item, keyTransformer));
                        }
                        else if (item != null && null != item.GetType().GetInterface("System.Collections.ICollection"))
                        {
                            newList.Add(TryConvertToExpandoIfCollection(item, keyTransformer));
                        }
                        else
                        {
                            newList.Add(item);
                        }
                    }
                    return newList;
                }
            }
            catch (Exception) { }
            return collection;
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
