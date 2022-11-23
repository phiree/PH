using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.ExcelExport;
using PHLibrary.ExcelExportExcelCreator;
using PHLibrary.Reflection.ArrayValuesToInstance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using PHLibrary.Reflection;
using static PHLibrary.Reflection.ColumnMapCreator;

namespace PHLibrary.ExcelExportExcelCreator.Tests
{
    [TestClass()]
    public class DataTableConverterTests
    {
        public DataTableConverterTests()
        {

        }
        [TestMethod()]
        public void ConvertTestWithDescription()
        {
            var studentList = new List<Student> { new Student { Name = "yf" } };
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
            var dataTable = converter.Convert(studentList, null, "F0",new List<ColumnDefine> {
                new ColumnDefine("age","年龄" ),
                 new ColumnDefine("name","姓名" )});
            Assert.AreEqual("年龄", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestWithDescriptionWithoutOrder()
        {
            var studentList = new List<Student2> { new Student2 { Name = "yf" } };
            DataTableConverter<Student2> converter = new DataTableConverter<Student2>();
            var dataTable = converter.Convert(studentList, null, "F0", new List<ColumnDefine> {
                new ColumnDefine("age","年龄" ),
                 new ColumnDefine("name","姓名" )});
            Assert.AreEqual("年龄", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestWithNullable()
        {
            var birthday = DateTime.Now.AddYears(-1);
            var studentList = new List<Student3> { new Student3 { Birthday = null }, new Student3 { Birthday = birthday } };
            DataTableConverter<Student3> converter = new DataTableConverter<Student3>();
            var dataTable = converter.Convert(studentList, null, "F0", new List<ColumnDefine> {
                  ColumnDefine.DatetimeColumn("birthday","生日","yyyy-mm-dd") 
                  });

            Assert.AreEqual("生日", dataTable.Columns[0].ColumnName);
            Assert.AreEqual(DBNull.Value, dataTable.Rows[0][0]);
            Assert.AreEqual(birthday, dataTable.Rows[1][0]);
        }

        [TestMethod()]
        public void ConvertTestWithAmountFormat()
        {

            DataTableConverter<Order1114> converter = new DataTableConverter<Order1114>();


            Assert.AreEqual((double)1, converter.Convert(
                new List<Order1114> { new Order1114 { Amount = 1234 } }
                , null
                , "F0"
                , new List<ColumnDefine> {  ColumnDefine.AmountColumn( "Amount"  ,"数量")}).Rows[0][0]
                );
            Assert.AreEqual(1.2, converter.Convert(new List<Order1114> { new Order1114 { Amount = 1234 } }, null, "F1", new List<ColumnDefine> { ColumnDefine.AmountColumn("Amount", "数量") }).Rows[0][0]);
            Assert.AreEqual(1.23, converter.Convert(new List<Order1114> { new Order1114 { Amount = 1234 } }, null, "F2", new List<ColumnDefine> { ColumnDefine.AmountColumn("Amount", "数量") }).Rows[0][0]);
            Assert.AreEqual(1.234, converter.Convert(new List<Order1114> { new Order1114 { Amount = 1234 } }, null, "F3", new List<ColumnDefine> { ColumnDefine.AmountColumn("Amount", "数量") }).Rows[0][0]);

        }

        public class Order1114
        {
            [CustomAmountFormat]
            [PropertyOrder(1)]
            public long Amount { get; set; }
        }

        public class Student
        {
            [PropertyOrder(3)]
            [Column("姓名")]
            public string Name { get; set; }
            [PropertyOrder(1)]
            public int Age { get; set; }
        }
        public class Student2
        {
            [PropertyOrder(1)]
            [Column("姓名")]
            public string Name { get; set; }
            [PropertyOrder(2)]
            public int Age { get; set; }
        }

        [TestMethod()]
        public void ConvertToTwoDimentioanlTest()
        {
            var table = CreateTable();

            DataTableConverter<Student> converter = new DataTableConverter<Student>();

            var newTable = converter.ConvertToTwoDimentioanl(table, new TwoDimensionalDefine("尺码", "SizeGuid", "数量"), Sort);

            Assert.AreEqual(6, newTable.Columns.Count);
            Assert.AreEqual("春装001", newTable.Rows[0][0]);
            Assert.AreEqual("红色", newTable.Rows[0][1]);
            Assert.AreEqual("2", newTable.Rows[0][2]);
            Assert.AreEqual("1", newTable.Rows[0][3]);
            Assert.AreEqual("3", newTable.Rows[0][4]);
            Assert.AreEqual("蓝色", newTable.Rows[1][1]);
            Assert.AreEqual("5", newTable.Rows[1][2]);
            Assert.AreEqual("4", newTable.Rows[1][3]);
            Assert.AreEqual("6", newTable.Rows[1][5]);
        }

        private System.Data.DataTable CreateTable()
        {
            var dataTable = new DataTable();
            DataColumn col1= new DataColumn("品名", typeof(string));
            col1.ExtendedProperties["columnDefine"]=ColumnDefine.GroupColumn ("name","品名");

                DataColumn col2 = new DataColumn("颜色", typeof(string)); 
                col2.ExtendedProperties["columnDefine"] = ColumnDefine.GroupColumn("color", "颜色");

            DataColumn col3 = new DataColumn("尺码", typeof(string));
            col3.ExtendedProperties["columnDefine"] = ColumnDefine.TwoDimensionalColumn("size", TwoDimensionalColumnType.Column);

            DataColumn col4 = new DataColumn("数量", typeof(int));
            col4.ExtendedProperties["columnDefine"] = ColumnDefine.TwoDimensionalColumn("amount", TwoDimensionalColumnType.Row);

            DataColumn col5 = new DataColumn("SizeGuid", typeof(string));
            col5.ExtendedProperties["columnDefine"] = ColumnDefine.OtherColumn("gguid", "品名guid");
            dataTable.Columns.Add(col1);
            dataTable.Columns.Add(col2);
            dataTable.Columns.Add(col3);
            dataTable.Columns.Add(col4);
            dataTable.Columns.Add(col5);

             
            dataTable.Rows.Add("春装001", "红色", "L", 1, "1");
            dataTable.Rows.Add("春装001", "红色", "M", 2, "2");
            dataTable.Rows.Add("春装001", "红色", "XL", 3, "3");
            dataTable.Rows.Add("春装001", "蓝色", "L", 4, "1");
            dataTable.Rows.Add("春装001", "蓝色", "M", 5, "2");
            dataTable.Rows.Add("春装001", "蓝色", "XXL", 6, "4");
            /*二维表：
            品名，  颜色      L M XL XXL
            春001  红色      1 2 3
            春001  蓝色      4 5     6
           
            */
            return dataTable;
        }

        private IList<TwoDimensionalX> Sort(IList<TwoDimensionalX> columns)
        {
            return new List<TwoDimensionalX> {new TwoDimensionalX("M","1")
                ,new TwoDimensionalX("L","2")
                ,new TwoDimensionalX("XL","3")
                ,new TwoDimensionalX( "XXL","4") };
        }

        public class Student3
        {


            [PropertyOrder(1)]
            public DateTime? Birthday { get; set; }
        }

        [TestMethod()]
        public void ConvertToTwoDimentioanlWithNullableTest()
        {
            var table = CreateTable();

            DataTableConverter<Student> converter = new DataTableConverter<Student>();

            var newTable = converter.ConvertToTwoDimentioanl(table, new TwoDimensionalDefine("尺码", "SizeGuid", "数量"), Sort);

            Assert.AreEqual(6, newTable.Columns.Count);
            Assert.AreEqual("春装001", newTable.Rows[0][0]);
            Assert.AreEqual("红色", newTable.Rows[0][1]);
            Assert.AreEqual("2", newTable.Rows[0][2]);
            Assert.AreEqual("1", newTable.Rows[0][3]);
            Assert.AreEqual("3", newTable.Rows[0][4]);
            Assert.AreEqual("蓝色", newTable.Rows[1][1]);
            Assert.AreEqual("5", newTable.Rows[1][2]);
            Assert.AreEqual("4", newTable.Rows[1][3]);
            Assert.AreEqual("6", newTable.Rows[1][5]);
        }

        
        [TestMethod()]
        public void GetFormatedAmountTest()
        {
            var dataTableConverter = new DataTableConverter<string>();
            Assert.AreEqual(1.23, dataTableConverter.GetFormatedAmount(1231, "F2"));
            Assert.AreEqual(1.231, dataTableConverter.GetFormatedAmount(1231, "F3"));
            Assert.AreEqual(1.2, dataTableConverter.GetFormatedAmount(1231, "F1"));
            Assert.AreEqual(1, dataTableConverter.GetFormatedAmount(1231, "f0"));


        }
    }
}