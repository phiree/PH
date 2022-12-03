using PHLibrary.ExcelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection
{
    public class ColumnMapCreator
    {


        public class ColumnDefineBuilder
        {


            IList<ColumnDefine> columns = new List<ColumnDefine>();
            public ColumnDefineBuilder AddNormalColumn(string property, string display, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.NormalColumn(property, display));
                return this;
            }
            public ColumnDefineBuilder AddImageColumn(string property, string display, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.ImageColumn(property, display));
                return this;
            }
            public ColumnDefineBuilder AddDatetimeColumn(string property, string display, string format, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.DatetimeColumn(property, display, format));
                return this;
            }
            public ColumnDefineBuilder AddTwoDimensionalColumns(string columnProperty, string columnGuidProperty, string rowProperty)
            {

                columns.Add(ColumnDefine.TwoDimensionalColumn(columnProperty, TwoDimensionalColumnType.Column));
                columns.Add(ColumnDefine.TwoDimensionalColumn(columnGuidProperty, TwoDimensionalColumnType.ColumnGuid));
                columns.Add(ColumnDefine.TwoDimensionalColumn(rowProperty, TwoDimensionalColumnType.Row));
                return this;
            }
            public ColumnDefineBuilder AddAmountColumn(string property, string display, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.AmountColumn(property, display));
                return this;
            }
            public ColumnDefineBuilder AddGroupColumn(string property, string display, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.GroupColumn(property, display));
                return this;
            }
            public ColumnDefineBuilder AddHiddenColumn(string property, string display, bool shouldAdd = true)
            {

                if (shouldAdd) columns.Add(ColumnDefine.HiddenColumn(property, display));
                return this;
            }
            public IList<ColumnDefine> Build()
            {
                return columns;
            }
        }
        public class ColumnDefine
        {
            /// <summary>
            /// 数据类属性名
            /// </summary>
            public string PropertyName { get; }
            /// <summary>
            /// excel表头中文名
            /// </summary>
            public string DisplayName { get; }
            /// <summary>
            /// 是否参与分组
            /// </summary>
            public bool IsInGroup { get; set; }
            /// <summary>
            /// 是否 隐藏
            /// </summary>
            public bool Hide { get; }
            /// <summary>
            /// 是否是图片
            /// </summary>
            public bool IsImage { get; }
            /// <summary>
            /// 时间格式化
            /// </summary>
            public string DatetimeFormat { get; }
            /// <summary>
            /// 二维列类型。
            /// </summary>
            public TwoDimensionalColumnType TwoDimensionalColumnType { get; }
            /// <summary>
            /// 是否需要自定义小数点精度
            /// </summary>
            public bool NeedFormatAmount { get; set; }


            private Type CalculateColumnType(Type propertyType)
            {
                if (IsImage) { return typeof(System.Drawing.Image); }
                if (!String.IsNullOrEmpty(DatetimeFormat)) { return typeof(DateTime); }
                if (NeedFormatAmount) { return typeof(double); }

                return propertyType;
            }
            public Type ColumnType
            {
                get
                {
                    if (IsImage) { return typeof(System.Drawing.Image); }
                    if (!String.IsNullOrEmpty(DatetimeFormat)) { return typeof(DateTime); }
                    if (NeedFormatAmount) { return typeof(double); }

                    return typeof(string);
                }
            }
            /// <summary>
            /// 图片列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="displayName"></param>
            /// <returns></returns>
            public static ColumnDefine ImageColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName, false, false, true, "", TwoDimensionalColumnType.None, false);
            }
            /// <summary>
            /// 图片列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <returns></returns>
            public static ColumnDefine ImageColumn(string propertyName)
            {
                return ColumnDefine.ImageColumn(propertyName, propertyName);//, false, false, true, "", TwoDimensionalColumnType.None, false);
            }
            /// <summary>
            /// 时间列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="displayName"></param>
            /// <param name="datetimeFormat">时间格式字符串</param>
            /// <returns></returns>
            public static ColumnDefine DatetimeColumn(string propertyName, string displayName, string datetimeFormat)
            {
                return new ColumnDefine(propertyName, displayName, false, false, false, datetimeFormat, TwoDimensionalColumnType.None, false);
            }
            /// <summary>
            /// 二维列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="twoDimensionalColumnType">二维列类型（列/行/guid）</param>
            /// <returns></returns>
            public static ColumnDefine TwoDimensionalColumn(string propertyName, TwoDimensionalColumnType twoDimensionalColumnType)
            {

                return new ColumnDefine(propertyName, propertyName, false, false, false, "", twoDimensionalColumnType, false);
            }
            /// <summary>
            /// 需要指定精度的数字
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="displayName"></param>
            /// <returns></returns>
            public static ColumnDefine AmountColumn(string propertyName, string displayName)

            {

                return new ColumnDefine(propertyName, displayName, false, false, false, "", TwoDimensionalColumnType.None, true);
            }
            /// <summary>
            /// 一般列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="displayName"></param>
            /// <returns></returns>
            public static ColumnDefine NormalColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName, false);
            }
            /// <summary>
            /// 二维数据 用来分组的列
            /// </summary>
            /// <param name="propertyName"></param>
            /// <param name="displayName"></param>
            /// <returns></returns>
            public static ColumnDefine GroupColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName, true);
            }
            public static ColumnDefine HiddenColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName, false, true, false, "", TwoDimensionalColumnType.None, false);
            }


            public ColumnDefine(string propertyName) : this(propertyName, propertyName, false) { }
            public ColumnDefine(string propertyName, string displayName) : this(propertyName, displayName, false) { }
            public ColumnDefine(string propertyName, string displayName, bool isInGroup)
                : this(propertyName, displayName, isInGroup, false, false, "", TwoDimensionalColumnType.None, false)
            { }
            public ColumnDefine(string propertyName, string displayName, bool isInGroup, bool hide, bool isImage, string format
                , TwoDimensionalColumnType twoDimensionalColumnType, bool isAmount)
            {
                IsInGroup = isInGroup;
                PropertyName = propertyName;
                NeedFormatAmount = isAmount;
                DisplayName = displayName;
                Hide = hide;
                IsImage = isImage;
                DatetimeFormat = format;
                TwoDimensionalColumnType = twoDimensionalColumnType;
            }

            public DataColumn CreateDataColumn(Type propertyType)
            {


                var dataColumn = new DataColumn(PropertyName, CalculateColumnType(propertyType));

                dataColumn.Caption = DisplayName;

                if (TwoDimensionalColumnType == TwoDimensionalColumnType.Row)
                {
                    dataColumn.DataType = typeof(int);
                }
                dataColumn.ExtendedProperties.Add("columnDefine", this);
                return dataColumn;
            }
        }
        public enum TwoDimensionalColumnType
        {
            None,
            /// <summary>
            /// 二维列
            /// </summary>
            Column,
            /// <summary>
            /// 二维列guid
            /// </summary>
            ColumnGuid,
            /// <summary>
            /// 二维行
            /// </summary>
            Row
        }
        //public class ColumnDefine
        //{
        //    public ColumnDefine(string propertyName, string displayName, object[] attributes)
        //    {
        //        PropertyName = propertyName;
        //        DisplayName = displayName;
        //        Attributes = attributes;
        //    }

        //    public string PropertyName { get; set; }
        //    public string DisplayName { get; set; }
        //    public object[] Attributes { get; set; } = new object[] { };


        //}
        public void CheckColulmnDefines(PropertyInfo[] propertyInfos, IList<ColumnDefine> columnDefines) //where T : class
        {



            foreach (var columnDefine in columnDefines)
            {
                if (!propertyInfos.Any(x => x.Name.ToLower() == columnDefine.PropertyName.ToLower()))
                {

                    throw new Exception($"没有找到对应的属性{columnDefine.PropertyName}");
                }
            }



        }


    }


}
