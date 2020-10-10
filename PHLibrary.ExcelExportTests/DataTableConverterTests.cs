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
        [TestMethod()]
        public void ConvertTestWithDescription()
        {
            var studentList = new List<Student> { new Student { Name = "yf" } };
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
            var dataTable = converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[1].ColumnName);
            Assert.AreEqual("Age", dataTable.Columns[0].ColumnName);
        }

        public class Student
        {
            [PropertyOrder(2)]
            [System.ComponentModel.DisplayName("姓名")]
            public string Name { get; set; }
            [PropertyOrder(1)]
            public int Age { get; set; }
        }
    }
}