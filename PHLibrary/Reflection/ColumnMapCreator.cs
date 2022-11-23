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

         
        public class ColumnDefine
        {
            /// <summary>
            /// 数据类属性名
            /// </summary>
             public string PropertyName { get;}
            /// <summary>
            /// excel表头中文名
            /// </summary>
            public string DisplayName { get; }
            /// <summary>
            /// 是否参与分组
            /// </summary>
            public bool IsInGroup { get;set;}
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
          
            public bool NeedFormatAmount { get;set;}

            public Type ColumnType
            {
                get {
                    if (IsImage) { return typeof(System.Drawing.Image); }
                    if (!String.IsNullOrEmpty( DatetimeFormat)) { return typeof(DateTime); }
                    if (NeedFormatAmount) { return typeof(double);}

                    return typeof(string);
                    }
                }

            public static ColumnDefine ImageColumn(string propertyName,string displayName) { 
                return new ColumnDefine(propertyName,displayName,false,false,true,"", TwoDimensionalColumnType.None,false);
                }
            public static ColumnDefine ImageColumn(string propertyName)
            {
                return   ColumnDefine.ImageColumn(propertyName, propertyName);//, false, false, true, "", TwoDimensionalColumnType.None, false);
            }
            public static ColumnDefine DatetimeColumn(string propertyName,string displayName,string datetimeFormat)
            {
                return new ColumnDefine(propertyName, displayName,false,false,false,datetimeFormat, TwoDimensionalColumnType.None,false);
            }
            public static ColumnDefine TwoDimensionalColumn(string propertyName, TwoDimensionalColumnType twoDimensionalColumnType)
            {
                 
                return new ColumnDefine(propertyName, propertyName,false, false,false, "", twoDimensionalColumnType,false);
            }
            public static ColumnDefine AmountColumn(string propertyName, string displayName)

            {

                return new ColumnDefine(propertyName, displayName,false, false, false, "", TwoDimensionalColumnType.None, true);
            }
            public static ColumnDefine OtherColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName, false);
            }
            public static ColumnDefine GroupColumn(string propertyName,string displayName) { 
                return new ColumnDefine(propertyName,displayName,true);
                }
            public static ColumnDefine HiddenColumn(string propertyName, string displayName)
            {
                return new ColumnDefine(propertyName, displayName,false,true,false,"", TwoDimensionalColumnType.None,false);
            }


            public ColumnDefine(string propertyName) : this(propertyName, propertyName, false) { }
            public ColumnDefine(string propertyName, string displayName) : this(propertyName, displayName, false) { }
            public ColumnDefine(string propertyName,string displayName,bool isInGroup)
                :this(propertyName,displayName, isInGroup,false, false,"",TwoDimensionalColumnType.None,false)
                {  }
            public ColumnDefine(string propertyName, string displayName,bool isInGroup, bool hide, bool isImage, string format
                , TwoDimensionalColumnType  twoDimensionalColumnType, bool isAmount)
            {
                IsInGroup=isInGroup;
                PropertyName = propertyName;
                 NeedFormatAmount=isAmount;
                DisplayName = displayName??propertyName;
                Hide = hide;
                IsImage = isImage;
                DatetimeFormat = format;
                TwoDimensionalColumnType=twoDimensionalColumnType;
            }
        
            public DataColumn CreateDataColumn() { 
                var dataColumn= new DataColumn(DisplayName,ColumnType);
                if(TwoDimensionalColumnType== TwoDimensionalColumnType.Row) { 
                    dataColumn.DataType=typeof(int);
                    }
                dataColumn.ExtendedProperties.Add("columnDefine",this);
                return dataColumn;
                }
            }
        public enum TwoDimensionalColumnType { 
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
        public void CheckColulmnDefines<T>(IList<ColumnDefine> columnDefines) //where T : class
        {


            var propertyInfos = typeof(T).GetProperties();
             
            foreach (var columnDefine in columnDefines)
            {
                if (!propertyInfos.Any(x => x.Name.ToLower() == columnDefine.PropertyName.ToLower()))
                {
 
                    throw new Exception($"类型{nameof(T)}没有找到对应的属性{columnDefine.PropertyName}");
                }
            }

           

        }
 
        
    }


}
