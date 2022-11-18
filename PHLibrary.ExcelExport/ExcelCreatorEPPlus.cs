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
using static PHLibrary.Reflection.ColumnMapCreator;
using System.Linq;
using PHLibrary.Reflection;

namespace PHLibrary.ExcelExport
{

    public class SheetData<T> { 
        /// <summary>
        /// sheet 对应的数据
        /// </summary>
        public IList<T> Data { get;set;}
        public string SheetName { get;set;}
        /// <summary>
        /// 需要展示的属性名称
        /// </summary>
        public IDictionary<string,string> PropertiesToDisplay {get;set; }
        public IList<IList<string>> SummaryDataForTopTable { get;set;}
        }


    /// <summary>
    ///创建表头合并的excel
    /// </summary>
    public class ExcelCreatorEPPlus : IExcelCreator
    {
        public Stream Create<T>(IList<SheetData<T>> sheetDatas,SortSize sortSize, string amountFormat ) { 
            
            IList<DataTable> tables=new List<DataTable>();
            foreach(var sheetData in sheetDatas) {
                var tableConvertor = new DataTableConverter<T>();
                var dataTable = tableConvertor.Convert(sheetData.Data, sortSize, amountFormat,sheetData.PropertiesToDisplay);
                dataTable.TableName = sheetData.SheetName;
                tables.Add(dataTable);
                dataTable.ExtendedProperties[SummaryDataForSheet] =sheetData.SummaryDataForTopTable;
            }
            

            return Create(tables);

        }

        const string SummaryDataForSheet= "SummaryData";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="propertiesToDisplay"></param>
        /// <param name="sheetName"></param>
        /// <param name="sortSize"></param>
        /// <param name="summaryData"></param>
        /// <param name="summaryTableBottomMargin"></param>
        /// <param name="cellStyleSettings"></param>
        /// <param name="amountFormat"></param>
        /// <param name="needExportImage"></param>
        /// <returns></returns>
        [Obsolete("已失效。只是为了向下兼容，后期会移除")]
        public Stream Create<T>(IList<T> data, IList<string> propertiesToDisplay, SortSize sortSize,
            IList<IList<string>> summaryData, int summaryTableBottomMargin,
            CellStyleSettings cellStyleSettings, string amountFormat, bool needExportImage, string sheetName="sheet1")
        { 
          var sheetDatas=  new List<SheetData<T>> { new SheetData<T>{ Data=data, PropertiesToDisplay=propertiesToDisplay, SheetName=sheetName, SummaryDataForTopTable=summaryData } };

            return Create(sheetDatas,sortSize,amountFormat);
            
            }
        /// <summary>
        /// 创建只包含一个Sheet的Excel
        ///     重载 包含多个Sheet的excel方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="propertiesToDisplay"></param>
        /// <param name="sheetName"></param>
        /// <param name="sortSize"></param>
        /// <param name="summaryData"></param>
        /// <param name="amountFormat"></param>
        /// <returns></returns>
        public Stream Create<T>(IList<T> data, IList<string> propertiesToDisplay, string sheetName, SortSize sortSize,
           IList<IList<string>> summaryData, string amountFormat )
        {
            var sheetDatas = new List<SheetData<T>> 
            { new SheetData<T> { Data = data, PropertiesToDisplay = propertiesToDisplay
                , SheetName = sheetName, SummaryDataForTopTable = summaryData } };

            return Create(sheetDatas, sortSize, amountFormat);

        }
        private Stream Create(IList<DataTable> datatables,  CellStyleSettings cellStyleSettings=null)
        {
            if (cellStyleSettings == null)
            {
                cellStyleSettings = new CellStyleSettings { BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin };
            }
            using (var excelPackage = new ExcelPackage())
            {
                for (int i = 0; i < datatables.Count; i++)// ar dataTable in dataToExport.Tables)
                {
                    var dataTable = datatables[i];

                    var columnTree =CreateColumnTree(dataTable);

                    string tablename = string.IsNullOrEmpty(dataTable.TableName) ? "sheet1" : dataTable.TableName;
                   
                    var sheet = excelPackage.Workbook.Worksheets.Add(tablename);
                    int summaryTableHeight = 0;
                    if( dataTable.ExtendedProperties[SummaryDataForSheet] is IList<IList<string>> summaryData)
                    { 
                    if (summaryData != null)
                    {
                        var summaryTableCreator = new SummaryTableCreator(sheet, 1);
                        summaryTableHeight = summaryTableCreator.Create(summaryData);
                    }
                    }
                    //create merged header cells
                    var headerCreateor = new ExceHeaderCreatorEPPlus(columnTree, sheet, cellStyleSettings.HeaderBackgroundColor, summaryTableHeight);
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

                roots.Add(new ColumnTreeNode {  Title = column.ColumnName });
            }
            columnTree.Roots = roots;
            return columnTree;
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
            //datetime 
            SetFormatForDateColumn(sheet, dataTable);

        }
        
        private void SetFormatForDateColumn(ExcelWorksheet sheet,DataTable dataTable) {
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns)
            {

                if (column.DataType == typeof(DateTime))
                {
                    var sheetColumn = sheet.Column(columnIndex);
                    var formatAttribute = ((ColumnDefine)column.ExtendedProperties["columnDefine"]).Attributes.FirstOrDefault(x=>x.GetType()==typeof(DateFormatAttribute));
                    string dateFormatString=formatAttribute!=null?((DateFormatAttribute) formatAttribute).DateFormatString: "yyyy/MM/dd HH:mm:ss";
                    
                    sheetColumn.Style.Numberformat.Format = dateFormatString;
                    sheetColumn.AutoFit();
                }
                columnIndex++;
            }
        }

        const int ImageWidth = 100;
        const int ImageHeight = 100;
        const int imageMargin = 3;
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
                        picture.SetPosition(startRow + i, imageMargin, j, imageMargin);
                        SetRowHeight(sheet, startRow + i + 1);
                        SetColumnWidth(sheet, j + 1);

                    }

                }
            }
        }

        private void SetRowHeight(ExcelWorksheet sheet, int rowIndex)
        {
            var row = sheet.Row(rowIndex);
            row.Height = (double)((ImageHeight + imageMargin * 2) * 72 / 96d);

        }
        private void SetColumnWidth(ExcelWorksheet sheet, int columnIdex)
        {
            var column = sheet.Column(columnIdex);
            column.Width = (double)((ImageWidth + imageMargin * 2 - 12 + 5) / 7d + 1);
            //https://stackoverflow.com/a/7902415/714883

        }


    }

}
