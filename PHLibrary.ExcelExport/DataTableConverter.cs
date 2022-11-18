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
using static PHLibrary.Reflection.ColumnMapCreator;

namespace PHLibrary.ExcelExportExcelCreator
{
    public delegate IList<TwoDimensionalX> SortSize(IList<TwoDimensionalX> x);
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

                    if (twoDimentinalColumnNames.Contains(column.ColumnName))
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

            IList<TwoDimensionalX> twoDimensionalColumns = columnsGroupByNoTwoDimensinal.SelectMany(x => x.ToList())
                .Select(x => new TwoDimensionalX(
                    x[twoDimentinalColumnNames.TwoDimensionalX.Name].ToString(),
                    x[twoDimentinalColumnNames.TwoDimensionalX.Guid] == null ? "" : x[twoDimentinalColumnNames.TwoDimensionalX.Guid].ToString())
                    )
                .Distinct()
                .ToList();


            var twoDimensionalColumnsOrdered = sortSize(twoDimensionalColumns.Distinct().ToList());
            var allColumns = new List<DataColumn>();

            foreach (var col in columnsBuilder.NotTwoDimensinalColumns)
            {
                allColumns.Add(col);
            }
            foreach (var col2 in twoDimensionalColumnsOrdered)
            {
                string colName = col2.Name;// col2.Guid!= string.Empty?col2.Guid:col2.Name;

                allColumns.Add(new DataColumn(colName.ToString()));
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
        /// 转换为datatable，方便eeplus调用 LoadRange（Datatable)方法。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sortSize"></param>
        /// <param name="amountFormat">小数点位数。F0，F1，F2，F3（数字表示小数点位数）</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTable Convert(IList<T> data, SortSize sortSize, string amountFormat ,IList<string> propertiesToDisplay)
        {


            var propertyNameMaps = new ColumnMapCreator().GetPropertyMaps<T>(propertiesToDisplay);

            var dataTable = new DataTable("Sheet1");
            var unOrderedColumns = new Dictionary<DataColumn, int>();
            for (int i = 0; i < propertyNameMaps.Count(); i++)// var name in memberNames)
            {
                var columnDefine = propertyNameMaps[i];
                int orderNo = i + 1;
                string name = columnDefine.PropertyName;
                string columnName = columnDefine.DisplayName;

                var attributes = columnDefine.Attributes;

                foreach (var attribute in attributes)
                {
                    if (attribute is PropertyOrderAttribute propertyOrder)
                    {
                        orderNo = propertyOrder.Order;
                    }

                }


                var columnType = new ColumnDataTypeDetermine<T>().GetPropertyType(data, name);
                //guess column type using first row of data
                var column = new DataColumn(columnName, Nullable.GetUnderlyingType(columnType) ?? columnType);
                column.Caption = name;

               
                column.ExtendedProperties.Add("columnDefine", columnDefine);

                unOrderedColumns.Add(column, orderNo);

            }
            //停用 propertyOrder
            foreach (var column in unOrderedColumns)//.OrderBy(x => x.Value))
            {
                dataTable.Columns.Add(column.Key);
            }
            foreach (T t in data)
            {
                var row = dataTable.NewRow();
                var finalColumns = new List<DataColumn>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    var columnDefine= (ColumnDefine)column.ExtendedProperties["columnDefine"];
                    string propertyName = columnDefine.PropertyName;
                    var value = Dynamic.InvokeGet(t, propertyName);
                    if (column.DataType == typeof(Image))
                    {
                        row[column.ColumnName] = DownloadImageAsync(value);
                    }
                    else
                    {
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        if (columnDefine.Attributes.Any(x=>x.GetType()== typeof( CustomAmountFormatAttribute)))
                        {

                            value = GetFormatedAmount((double)value, amountFormat);
                        }
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

        /// <summary>
        /// 系统的金额都是 厘，
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public double GetFormatedAmount(double amount, string amountFormat)
        {
            int digits = 0;
            var yuan = amount * 1.0 / 1000;


            switch (amountFormat.ToLower())
            {
                case "f0":

                    break;
                case "f1": digits = 1; break;
                case "f2": digits = 2; break;
                case "f3": digits = 3; break;
                default: throw new Exception("错误的格式：" + amountFormat);
            }
            var result = Math.Round(yuan, digits);
            return result;

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
}
