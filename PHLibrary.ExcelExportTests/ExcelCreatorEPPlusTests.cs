using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.ExcelExport;
using System;
using System.Collections.Generic;
using System.Data;
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

            var stream = excelCreator.Create(CreateDemoDataSet(2, 130, 300));
            string fileName = "createdexcel_from_dataset" + Guid.NewGuid() + ".xlsx";
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
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
                int colAmount = 10;
                for (int colIndex = 0; colIndex < columnAmount; colIndex++)
                {
                    table.Columns.Add(new DataColumn("col_" + colIndex));

                }
                for (int r = 0; r < rowAmount; r++)
                {

                    var row = table.NewRow();
                    for (int col = 0; col < columnAmount; col++)
                    {
                        row[col] = $"c{col}_r{r}";
                    }
                    table.Rows.Add(row);
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;

        }


    }
}