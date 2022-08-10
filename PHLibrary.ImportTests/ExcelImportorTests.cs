using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Import;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Import.Tests
{
    [TestClass()]
    public class ExcelImportorTests
    {
        [TestMethod()]
        public void ImportTest()
        {
            var stream = System.IO.File.OpenRead(Environment.CurrentDirectory + "\\files\\importfile.xlss");
            var importor =new Import.ExcelImportor<Student>();
            importor.Import(stream, null);
        }
    }
    public class Student
    { 
     public int No { get; set; }
        public string Name { get; set; }

    }
}