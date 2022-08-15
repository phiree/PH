using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Arithmetic.TreeToRectangle;
using PHLibrary.ExcelExport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PHLibrary.ExcelExport.Tests
{
    [TestClass()]
    public class ExcelCreatorEPPlusTests
    {
        public class TestItem { 
            public string Name { get; set; }
            public int Age { get;set;}
            }
        [TestMethod()]
        public void CreateForEmptyList()
        { 
            var list=new List<TestItem>();
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
           var stream  =  excelCreator.Create(list);
            using (var file = new FileStream("CreateForEmptyList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        [TestMethod()]
        public void CreateForNotEmptyDynamicList()
        {
            var list = new List<dynamic>{ new{Age=1,Name="name1" },new{ Age=2,Name="name2"} };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForNotEmptyDynamicList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        [TestMethod()]
        public void CreateForEmptyDynamicList()
        {
            Assert.IsTrue(true);
            return;
            var list = new List<dynamic> {  };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForEmptyDynamicList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }

        [TestMethod()]
        public void CreateForWithFormat()
        {
            var list = new List<dynamic> {
                new { Price = 1, Name = "name1" }
            , new { Price = 10, Name = "name2" }
             , new { Price = 101, Name = "name3" }
              , new { Price = 1000, Name = "name4" },
                 new { Price = 10000, Name = "name5" }
            , new { Price = 100000, Name = "name6" }
           
              };

            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            ColumnTree tree=new ColumnTree { 
                 Roots=new List<ColumnTreeNode> { 
                      new ColumnTreeNode{ Title="价格1", Format="¥#,##0.00;¥-#,##0.00" },
                      new ColumnTreeNode{ Title="名称1" }
                     }
                };
            var stream = excelCreator.Create(list,tree,new CellStyleSettings { HeaderBackgroundColor= System.Drawing.Color.AliceBlue,  BorderStyle= OfficeOpenXml.Style.ExcelBorderStyle.Dotted, HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Left });
            using (var file = new FileStream("CreateForWithFormat" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }



        [TestMethod()]
        public void CreateForWithWidth()
        {
            var list = new List<dynamic> {
                new { Price = 1, Name = "name1" }
            , new { Price = 10, Name = "name2" }
             , new { Price = 101, Name = "name3" }
              , new { Price = 1000, Name = "name4" },
                 new { Price = 10000, Name = "name5" }
            , new { Price = 987263541.23, Name = "name6" }

              };

            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            ColumnTree tree = new ColumnTree
            {
                Roots = new List<ColumnTreeNode> {
                      new ColumnTreeNode{ Title="价格1" },
                      new ColumnTreeNode{ Title="名称1", ColumnWidth=100}
                     }
            };
            var stream = excelCreator.Create(list, tree, new CellStyleSettings { HeaderBackgroundColor = System.Drawing.Color.AliceBlue, BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Dotted, HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left });
            using (var file = new FileStream("CreateForWithWidth" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }



        [TestMethod()]
        public void CreateForNotEmptyList()
        {
            var list = new List<TestItem> {new TestItem{ Age=1,Name="name1" },new TestItem{ Age=2,Name="name2"} };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForNotEmptyList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        [TestMethod()]
        public void CreateFromDataSetTest()
        {
            ExcelCreatorEPPlus excelCreator
                = new ExcelCreatorEPPlus();
            Stopwatch watch = Stopwatch.StartNew();
            
            var stream = excelCreator.Create(CreateDemoDataSet(2, 130, 100));
         
            using (var file = new FileStream("createdexcel_from_dataset" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
            Console.WriteLine("单元格不画框用时:" + watch.Elapsed);
            watch.Restart();
          
            var   stream2 = excelCreator.Create(CreateDemoDataSet(2, 130, 100),    new CellStyleSettings {  BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin});
            

            using (var file = new FileStream("createdexcel_from_dataset_with_border_" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream2, file);

            }
            Console.WriteLine("  单元格画框用时:" + watch.Elapsed);
           
           


        }

        /// <summary>
        /// 原因: 数智沙盘新栅格2020-12月数据5w条 需要6.4minuts, why?
        /// </summary>
        [TestMethod()]
        public void ExportPerformanceTest()
        {
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var config = FluentExcel.Excel.Setting.For<数值沙盘新栅格报表>();
           
            Stream stream = excelCreator.Create(CreateDemoDataSet(2, 130, 100));
            FluentExcel.Excel.Load<数值沙盘新栅格报表>(stream, 2);
        }
        public class 数值沙盘新栅格报表
        { 
        
        }
        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
        private DataSet CreateDemoDataSet(int tableAmount, int columnAmount, int rowAmount)
        {
            DataSet dataSet = new DataSet();
            for (int i = 0; i < tableAmount; i++)
            {
               
                var table = new DataTable("table_" + i);
               
                for (int colIndex = 0; colIndex < columnAmount; colIndex++)
                {
                    string title = "col_" + colIndex;
                    table.Columns.Add(new DataColumn(title));
                   
                }
                for (int r = 0; r < rowAmount; r++)
                {

                    var row = table.NewRow();
                    for (int col = 0; col < columnAmount; col++)
                    {
                        row[col] = $"12.312345";
                    }
                    table.Rows.Add(row);
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;

        }


    }
}