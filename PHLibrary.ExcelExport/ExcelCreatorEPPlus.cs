using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using PHLibrary.ExcelExportExcelCreator;
using System.Drawing;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    ///创建表头合并的excel
    /// </summary>
    public class ExcelCreatorEPPlus : IExcelCreator
    {

        public Stream Create<T>(IList<T> data, IDictionary<string, string> propertyNameMaps = null, CellStyleSettings cellStyleSettings = null,SortSize sortSize=null)

        {
            var tableConvertor = new DataTableConverter<T>();
            var dataTable = tableConvertor.Convert(data, propertyNameMaps,sortSize:sortSize);

            return Create(dataTable, cellStyleSettings);


        }
        public System.IO.Stream Create(DataTable dataTable, CellStyleSettings cellStyleSettings = null)
        {
            return Create(dataTable, CreateColumnTree(dataTable), cellStyleSettings);
        }
        public System.IO.Stream Create(DataSet dataset, ColumnTree columnTree, CellStyleSettings cellStyleSettings = null)
        {
            return Create(FetchFrom(dataset), new List<ColumnTree> { columnTree }, cellStyleSettings);
        }
        public System.IO.Stream Create(DataTable dataTable, ColumnTree columnTree, CellStyleSettings cellStyleSettings = null)
        {
            return Create(new List<DataTable> { dataTable }, new List<ColumnTree> { columnTree }, cellStyleSettings);
        }
        public Stream Create(DataSet dataToExport, CellStyleSettings cellStyleSettings = null)
        {
            return Create(dataToExport, CreateColumnTrees(dataToExport), cellStyleSettings);
        }
        public Stream Create(DataSet dataToExport, IList<ColumnTree> columnTrees, CellStyleSettings cellStyleSettings = null)
        {
            return Create(FetchFrom(dataToExport), columnTrees, cellStyleSettings);

        }
        private IList<DataTable> FetchFrom(DataSet ds)
        {
            var tables = new List<DataTable>();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                tables.Add(ds.Tables[i]);
            }
            return tables;
        }
        private Stream Create(IList<DataTable> datatables, IList<ColumnTree> columnTrees, CellStyleSettings cellStyleSettings = null)
        {
            if (cellStyleSettings == null)
            {
                cellStyleSettings = new CellStyleSettings { BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin };
            }

            if (datatables.Count != columnTrees.Count)
            {
                throw new Exception($"表头数量[{columnTrees.Count}]和表格数量[{datatables.Count}]不一致.");
            }
            using (var excelPackage = new ExcelPackage())
            {
                for (int i = 0; i < datatables.Count; i++)// ar dataTable in dataToExport.Tables)
                {
                    var dataTable = datatables[i];
                    string tablename = string.IsNullOrEmpty(dataTable.TableName) ? "sheet1" : dataTable.TableName;
                    var columnTree = columnTrees[i];
                    var sheet = excelPackage.Workbook.Worksheets.Add(tablename);
                    //create merged header cells
                    var headerCreateor = new ExceHeaderCreatorEPPlus(columnTree, sheet, cellStyleSettings.HeaderBackgroundColor);
                    IList<string> columnFormats;
                    int headerHeight = headerCreateor.CreateHeader(out columnFormats);
                    //create body 
                    FillSheetEpplusWithLoadRange(sheet, datatables[i], headerHeight, columnFormats, cellStyleSettings);



                    LoadPictures(sheet, dataTable, headerHeight);
                }
                Stream stream = new MemoryStream();
                excelPackage.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }


        private ColumnTree CreateColumnTree(DataTable table)
        {
            ColumnTree columnTree = new ColumnTree();
            var roots = new List<ColumnTreeNode>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                roots.Add(new ColumnTreeNode { Title = column.ColumnName });
            }
            columnTree.Roots = roots;
            return columnTree;
        }
        private List<ColumnTree> CreateColumnTrees(DataSet dataSet)
        {
            var columnTrees = new List<ColumnTree>();
            for (int ti = 0; ti < dataSet.Tables.Count; ti++)
            {
                columnTrees.Add(CreateColumnTree(dataSet.Tables[ti]));
            }
            return columnTrees;
        }
        private void FillSheetEpplusWithLoadRange(ExcelWorksheet sheet, DataTable dataTable, int startRow, IList<string> columnFormats)
        {
            FillSheetEpplusWithLoadRange(sheet, dataTable, startRow, columnFormats, null);
        }


        private void FillSheetEpplusWithLoadRange(ExcelWorksheet sheet, DataTable dataTable, int startRow, IList<string> columnFormats, CellStyleSettings cellStyleSettings)
        {

            //A workbook must have at least on cell, so lets add one... 

            int rows = dataTable.Rows.Count;
            int columns = dataTable.Columns.Count;
            if (rows == 0)
            {
                rows = 1;
            }
            //fill data
            var cells = sheet.Cells[startRow + 1, 1, rows, columns];


            cells.LoadFromDataTable(dataTable, false);

            //format
            for (int i = 0; i < columnFormats.Count; i++)// format in columnFormats)
            {
                string format = columnFormats[i];
                if (!string.IsNullOrEmpty(format))
                {

                    var columnCells = sheet.Cells[startRow + 1, i + 1, startRow + rows, i + 1];
                    //columnCells
                    columnCells.Style.Numberformat.Format = format;
                    if (0 == sheet.Column(i + 1).Width)
                    {
                        columnCells.AutoFitColumns();
                    }
                }
            }
            //style

            var bodyCells = sheet.Cells[startRow + 1, 1, startRow + rows, columns];
            if (cellStyleSettings != null)
            {

                bodyCells.Style.Border.BorderAround(cellStyleSettings.BorderStyle);
                bodyCells.Style.HorizontalAlignment = cellStyleSettings.HorizontalAlignment;

                //  bodyCells.AutoFitColumns();
            }
            //image

        }

        const int ImageWidth=100;
        const int ImageHeight=100;
        const int imageMargin=3;
        private void LoadPictures(ExcelWorksheet sheet, DataTable dataTable, int startRow)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                for (var j = 0; j < dataTable.Columns.Count; j++)
                {
                    var dataCell = row[j];
                    if (dataCell is System.Drawing.Image)

                    {
                        var cell = sheet.Cells[startRow + i + 1, j + 1];
                        cell.Value = "";

                        var picture = sheet.Drawings.AddPicture($"pic_{i}_{j}", dataCell as System.Drawing.Image);

                        
                        picture.SetSize(ImageWidth, ImageHeight);
                        //picture.EditAs=OfficeOpenXml.Drawing.eEditAs.TwoCell;
                        picture.SetPosition(startRow + i,imageMargin, j,imageMargin);
                        SetRowHeight(sheet, startRow + i + 1);
                        SetColumnWidth(sheet,j+1);

                    }

                }
            }
        }
        private static double GetMeasureFromPixels(int pixelSize)
        {

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiY = graphics.DpiY;
                return pixelSize * (72.0 * (1 / dpiY));
            }

        }
        private static double GetMeasureFromPixels2(int pixelSize)
        {

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiY = graphics.DpiY;
                return pixelSize * (1 / 72.0) * dpiY;
            }

        }
        private static int GetHeightInPixels(ExcelRange cell)
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiY = graphics.DpiY;
                return (int)(cell.Worksheet.Row(cell.Start.Row).Height * (1 / 72.0) * dpiY);
            }
        }

        public static float MeasureString(string s, Font font)
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                return g.MeasureString(s, font, int.MaxValue, StringFormat.GenericTypographic).Width;
            }
        }

        private static int GetWidthInPixels(ExcelRange cell)
        {
            double columnWidth = cell.Worksheet.Column(cell.Start.Column).Width;
            Font font = new Font(cell.Style.Font.Name, cell.Style.Font.Size, FontStyle.Regular);

            double pxBaseline = Math.Round(MeasureString("1234567890", font) / 10);

            return (int)(columnWidth * pxBaseline);
        }
        private static double GetWidth(int pix)
        {
            return (pix - 12 + 5) / 7d + 1;
        }

        private static double GetHeight(int pix)
        {
            return pix * 72 / 96d;
        }
        private void SetRowHeight(ExcelWorksheet sheet, int rowIndex)
        {
            var row = sheet.Row(rowIndex);
            row.Height = GetHeight(ImageHeight+imageMargin*2);

        }
        private void SetColumnWidth(ExcelWorksheet sheet, int columnIdex)
        {
            var column = sheet.Column(columnIdex);
            column.Width = GetWidth(ImageWidth+imageMargin*2);
            //https://stackoverflow.com/a/7902415/714883

        }

        public Stream Create<T>(IList<T> data, ColumnTree tree, CellStyleSettings cellStyleSettings = null,SortSize sortSize=null)
        {
            var dataTable = new DataTableConverter<T>().Convert(data,sortSize:sortSize);
            return Create(dataTable, tree, cellStyleSettings);
        }
    }

}
