using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.ExcelExportExcelCreator;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.ExcelExportExcelCreator.Tests
{
    [TestClass()]
    public class DataTableConverterTests
    {
        [TestMethod()]
        public void ConvertTest()
        {
            var studentList = new List<Student> { new Student { Name = "yf" } };
            DataTableConverter<Student> converter = new DataTableConverter<Student>();
          var dataTable=  converter.Convert(studentList, null);
            Assert.AreEqual("姓名", dataTable.Columns[0].ColumnName);
        }
        public class Student { 

        [System.ComponentModel.Description("姓名")]
        public string Name { get; set; }
        }
    }
}