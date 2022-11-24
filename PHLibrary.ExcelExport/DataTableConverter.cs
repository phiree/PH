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
using System.Reflection;

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
            /// 分组列 
            /// </summary>
            public IList<DataColumn> GroupingColumns { get; private set; } = new List<DataColumn>();
            /// <summary>
            /// 其他列：数据汇总，只取第一个值
            /// </summary>
            public IList<DataColumn> OtherColumns { get; private set; } = new List<DataColumn>();
            /// <summary>
            ///  二维数据的列
            /// </summary>
            public IList<DataColumn> TwoDimensinalColumns { get; private set; } = new List<DataColumn>();
            public void Build(DataTable originalTable, TwoDimensionalDefine twoDimentinalColumnNames)
            {


                foreach (DataColumn column in originalTable.Columns)
                {
                    var columnDefine = (ColumnDefine)column.ExtendedProperties["columnDefine"];

                    if (twoDimentinalColumnNames.Contains(column.ColumnName))
                    {
                        TwoDimensinalColumns.Add(column);
                    }
                    else
                    {
                        if (columnDefine.IsInGroup)
                        {
                            GroupingColumns.Add(column);
                        }
                        else
                        {
                            OtherColumns.Add(column);
                        }
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
                         .GroupBy(x => GetGroupBy(columnsBuilder.GroupingColumns.Select(c => c.ColumnName), x))
                         //.GroupBy(x=>new{品名= x.Field<string>("品名"),颜色=x.Field<string>("颜色") })
                         .Select((g, v) =>
                         {
                             var g2 = g.ToList().GroupBy
                              (k => GetGroupBy(
                                  new string[] { twoDimentinalColumnNames.TwoDimensionalX.Guid, twoDimentinalColumnNames.TwoDimensionalX.Name }
                                   , k));

                             var sumedRows = new List<DataRow>();
                             foreach (var groupByTwoDimensionalColumn in g2)
                             {


                                 var row = originalTable.NewRow();
                                 int totalY = 0;
                                 foreach (DataColumn col in originalTable.Columns)
                                 {
                                     var columnDefine = (ColumnDefine)col.ExtendedProperties["columnDefine"];
                                     //其他列 取第一个
                                     if (columnsBuilder.OtherColumns.Any(x => x.ColumnName == col.ColumnName)

                                     )
                                     {
                                         row[col] = g.ToList()[0][col];

                                     }
                                     //一级group列，取key的值
                                     else if (columnsBuilder.GroupingColumns.Any(x => x.ColumnName == col.ColumnName))
                                     {
                                         row[col] = ((IDictionary<string, object>)g.Key)[col.ColumnName];
                                     }
                                     //
                                     //二级group列（二维列名称和guid
                                     else if (twoDimentinalColumnNames.TwoDimensionalX.Name == col.ColumnName
                                    || twoDimentinalColumnNames.TwoDimensionalX.Guid == col.ColumnName
                                     )
                                     {
                                         row[col] = ((IDictionary<string, object>)groupByTwoDimensionalColumn.Key)[col.ColumnName];
                                     }
                                     //二维 Y 列，取汇总值
                                     else if (columnDefine.TwoDimensionalColumnType == TwoDimensionalColumnType.Row)
                                     {
                                         row[col] = groupByTwoDimensionalColumn.ToList().Select(x => x[col]).Sum(x => (int)x);
                                     }

                                 }
                                 sumedRows.Add(row);

                             }

                             return CreateGroup(sumedRows, g.Key);
                             //   return g;
                             ;
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

            bool twoDimensinalColumnsAdded = false;
            foreach (DataColumn col in originalTable.Columns)
            {
                if (columnsBuilder.GroupingColumns.Any(x => x.ColumnName == col.ColumnName))
                {
                    allColumns.Add(col);
                    continue;
                }

                if (columnsBuilder.OtherColumns.Any(x => x.ColumnName == col.ColumnName))
                {
                    allColumns.Add(col);
                    continue;
                }
                if (!twoDimensinalColumnsAdded)
                {
                    foreach (var col2 in twoDimensionalColumnsOrdered)
                    {

                        string colName = col2.Name;// col2.Guid!= string.Empty?col2.Guid:col2.Name;
                        string colCaption = col2.Name;
                        if (!string.IsNullOrEmpty(col2.Guid))
                        {
                            colName = col2.Guid;
                            colCaption = col2.Name;

                        }
                        var twoDimensionalColumn = new DataColumn(colName);
                        twoDimensionalColumn.Caption = colCaption;
                        twoDimensionalColumn.ExtendedProperties.Add("isGuid", true);
                        allColumns.Add(twoDimensionalColumn);

                        // allColumns.Add(new DataColumn(col2.Guid.ToString()));
                    }
                    twoDimensinalColumnsAdded = true;
                }
            }




            DataTable newt = new DataTable();

            foreach (var col in allColumns)
            {
                DataColumn newCol = new DataColumn(col.ColumnName, col.DataType);
                newCol.Caption=col.Caption;
                newCol.ExtendedProperties["columnDefine"] = col.ExtendedProperties["columnDefine"];
                newt.Columns.Add(newCol);// new DataColumn(col.ColumnName, col.DataType)); ;
            }


            foreach (var group in columnsGroupByNoTwoDimensinal)
            {
                var row = newt.NewRow();
                foreach (DataColumn col in newt.Columns)
                {
                    //分组列数据填充
                    if (((IDictionary<string, object>)group.Key).ContainsKey(col.ColumnName))
                    {
                        row[col.ColumnName] = ((IDictionary<string, object>)group.Key)[col.ColumnName];
                    }
                    //二维列数据填充
                    foreach (var r in group.ToList())
                    {
                        if (r[twoDimentinalColumnNames.TwoDimensionalX.Guid]==col.ColumnName||   r[twoDimentinalColumnNames.TwoDimensionalX.Name].ToString() == col.ColumnName)
                        {
                            row[col.ColumnName] = r[twoDimentinalColumnNames.YName];

                        }
                        else if (columnsBuilder.OtherColumns.Any(x => x.ColumnName == col.ColumnName))
                        {
                            row[col.ColumnName] = r[col.ColumnName];
                        }
                    }
                }
                newt.Rows.Add(row);
            }
            return newt;

        }
        private IGrouping<TKey, TElement> CreateGroup<TKey, TElement>(IEnumerable<TElement> theSeqenceToGroup, TKey valueForKey)
        {
            return theSeqenceToGroup.GroupBy(stg => valueForKey).FirstOrDefault();
        }
        private IList<DataRow> MergeOtherColumns(IGrouping<ExpandoObject, DataRow> groupResults)
        {

            throw new Exception();
            // foreach(var key in groupResults.Key)
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
        public static TwoDimensionalDefine GetTwoDimensionalColumns(IList<ColumnDefine> columnDefines) //where T : class
        {
            if (!columnDefines.Any(x => x.TwoDimensionalColumnType == TwoDimensionalColumnType.Column)) { return null; }

            string twoDimensionalColumn_X = columnDefines.Single(x => x.TwoDimensionalColumnType == TwoDimensionalColumnType.Column).PropertyName;

            var xguid = columnDefines.FirstOrDefault(x => x.TwoDimensionalColumnType == TwoDimensionalColumnType.ColumnGuid);
            string twoDimensionalColumn_XId = xguid == null ? twoDimensionalColumn_X : xguid.PropertyName;// columnDefines.Single(x => x.TwoDimensionalColumnType == TwoDimensionalColumnType.ColumnGuid).PropertyName;
            string twoDimensionalColumn_Y = columnDefines.FirstOrDefault(x => x.TwoDimensionalColumnType == TwoDimensionalColumnType.Row).PropertyName;


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
        public DataTable Convert(IList<T> data, SortSize sortSize, string amountFormat, IList<ColumnDefine> columnDefines)
        {


            new ColumnMapCreator().CheckColulmnDefines<T>(columnDefines);

            var dataTable = new DataTable("Sheet1");

            foreach (var columnDefine in columnDefines)// var name in memberNames)
            {
                dataTable.Columns.Add(columnDefine.CreateDataColumn());
            }

            foreach (T t in data)
            {
                var row = dataTable.NewRow();

                foreach (DataColumn column in dataTable.Columns)
                {
                    var columnDefine = (ColumnDefine)column.ExtendedProperties["columnDefine"];
                    string propertyName = columnDefine.PropertyName;
                    //                    var value = Dynamic.InvokeGet(t, propertyName);
                    var value = t.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).GetValue(t, null);
                    if (column.DataType == typeof(Image))
                    {
                        if (value is string imageUrl)
                        {
                            value = DownloadImageAsync(imageUrl);
                        }
                    }
                    else if (column.DataType == typeof(double))
                    {
                        if (columnDefine.NeedFormatAmount)
                        {
                            value = GetFormatedAmount(System.Convert.ToDouble(value), amountFormat);
                        }
                    }
                    else
                    {
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }


                    }
                    row[column.ColumnName] = value;
                }
                dataTable.Rows.Add(row);
            }

            var twoDimensionalColumns = GetTwoDimensionalColumns(columnDefines);
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
