using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PHLibrary.ExcelExportTests
{
    public class DemoData
    {
        public static   DataSet CreateDemoDataSet(int tableAmount, int columnAmount, int rowAmount)
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
                        row[col] = $"{i}_{r}_{col}";
                    }
                    table.Rows.Add(row);
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;

        }

    }
}
