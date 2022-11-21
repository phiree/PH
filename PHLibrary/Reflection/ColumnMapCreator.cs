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
             public string PropertyName { get;}
            public string DisplayName { get; }
            public bool Hide { get; }
            public bool IsImage { get; }
            public string DatetimeFormat { get; }
            public TwoDimensionalColumnType TwoDimensionalColumnType { get; }
          
            public bool IsAmount { get;set;}

            public Type ColumnType
            {
                get {
                    if (IsImage) { return typeof(System.Drawing.Image); }
                    if (!String.IsNullOrEmpty( DatetimeFormat)) { return typeof(DateTime); }
                    if (IsAmount) { return typeof(double);}

                    return typeof(string);
                    }
                }

            public static ColumnDefine ImageColumn(string propertyName,string displayName) { 
                return new ColumnDefine(propertyName,displayName,false,true,"", TwoDimensionalColumnType.None,false);
                }
            public static ColumnDefine ImageColumn(string propertyName)
            {
                return ImageColumn(propertyName,propertyName);
            }
            public static ColumnDefine DatetimeColumn(string propertyName,string displayName,string datetimeFormat)
            {
                return new ColumnDefine(propertyName, displayName,false,false,datetimeFormat, TwoDimensionalColumnType.None,false);
            }
            public static ColumnDefine TwoDimensionalColumn(string propertyName, TwoDimensionalColumnType twoDimensionalColumnType)
            {
                bool isAmount=twoDimensionalColumnType== TwoDimensionalColumnType.Row;
                return new ColumnDefine(propertyName, propertyName, false,false, "", twoDimensionalColumnType,isAmount);
            }
            public static ColumnDefine AmountColumn(string propertyName, string displayName)

            {

                return new ColumnDefine(propertyName, displayName, false, false, "", TwoDimensionalColumnType.None, true);
            }
            public ColumnDefine(string propertyName) : this(propertyName, propertyName) { }
            public ColumnDefine(string propertyName,string displayName)
                :this(propertyName,displayName,false,false,"",TwoDimensionalColumnType.None,false)
                {  }
            public ColumnDefine(string propertyName, string displayName, bool hide, bool isImage, string format, TwoDimensionalColumnType  twoDimensionalColumnType, bool isAmount)
            {
                
                PropertyName = propertyName;
                 IsAmount=isAmount;
                DisplayName = displayName??propertyName;
                Hide = hide;
                IsImage = isImage;
                DatetimeFormat = format;
                TwoDimensionalColumnType=twoDimensionalColumnType;
            }
        
            public DataColumn CreateDataColumn() { 
                var dataColumn= new DataColumn(DisplayName,ColumnType);
                dataColumn.ExtendedProperties.Add("columnDefine",this);
                return dataColumn;
                }
            }
        public enum TwoDimensionalColumnType { 
            None,
            Column,
            ColumnGuid,
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
