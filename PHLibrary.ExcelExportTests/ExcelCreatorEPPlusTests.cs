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
            
            var config = FluentExcel.Excel.Setting.For<数值沙盘新栅格报表>();
           
            Stream stream = null;
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