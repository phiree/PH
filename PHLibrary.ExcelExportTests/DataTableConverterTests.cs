using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.ExcelExportExcelCreator;
using PHLibrary.Reflection.ArrayValuesToInstance;
using System;
using System.Collections.Generic;
using System.Text;

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
            var dataTable = converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
            Assert.AreEqual("Age", dataTable.Columns[0].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestWithDescriptionWithoutOrder()
        {
            var studentList = new List<Student2> { new Student2 { Name = "yf" } };
            DataTableConverter<Student2> converter = new DataTableConverter<Student2>();
            var dataTable = converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("Age", dataTable.Columns[1].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestForDynamic()
        {
            var studentList = new List<dynamic> { new { Name = "name1", Age = 18 } };
            DataTableConverter<dynamic> converter = new DataTableConverter<dynamic>();
            var dataTable = converter.Convert(studentList, new Dictionary<string, string> { {"Name","姓名" },{ "Age","年龄"} });
            Assert.AreEqual("姓名", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("年龄", dataTable.Columns[1].ColumnName);
        }
        [TestMethod()]
        public void ConvertTestForDynamicNoMapping()
        {
            var studentList = new List<dynamic> { new { Name = "name1", Age = 18 } };
            DataTableConverter<dynamic> converter = new DataTableConverter<dynamic>();
            var dataTable = converter.Convert(studentList);
            Assert.AreEqual("Name", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("name1", dataTable.Rows[0][0].ToString());
            Assert.AreEqual("Age", dataTable.Columns[1].ColumnName);
        }
        [TestMethod()]
           public void ConvertTestForDynamicNoRowsWithColumnsMap()
        {
            var studentList = new List<dynamic>();// { new { Name = "name1", Age = 18 } };
            DataTableConverter<dynamic> converter = new DataTableConverter<dynamic>();
            var dataTable = converter.Convert(studentList, new Dictionary<string, string> { { "Name", "姓名" }, { "Age", "年龄" } });
            Assert.AreEqual(2, dataTable.Columns.Count);
            Assert.AreEqual(0, dataTable.Rows.Count);
        }
        [TestMethod()]
        public void ConvertTestForDynamicNoRowsNoColumnsMap()
        {
            var studentList = new List<dynamic>();// { new { Name = "name1", Age = 18 } };
            DataTableConverter<dynamic> converter = new DataTableConverter<dynamic>();
            var dataTable = converter.Convert(studentList);
            Assert.AreEqual(0, dataTable.Columns.Count);
            Assert.AreEqual(0, dataTable.Rows.Count);
        }
        [TestMethod()]
        public void ConvertTestForTypedListWith0Rows()
        {
            var studentList = new List<Student>();// { new Student { Name = "name1", Age = 18 } };
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
            var dataTable = converter.Convert(studentList);//, new Dictionary<string, string> { { "Name", "姓名" }, { "Age", "年龄" } });
            Assert.AreEqual("Age", dataTable.Columns[0].ColumnName);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
        }

        public class Student
        {
            [PropertyOrder(2)]
            [System.ComponentModel.DisplayName("姓名")]
            public string Name { get; set; }
            [PropertyOrder(1)]
            public int Age { get; set; }
        }
        public class Student2
        {
           
            [System.ComponentModel.DisplayName("姓名")]
            public string Name { get; set; }
            
            public int Age { get; set; }
        }
    }
}