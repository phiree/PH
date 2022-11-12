using System.Collections.Generic;
using System.Linq;
using PHLibrary.Arithmetic.TreeToRectangle;
using OfficeOpenXml;
using System.Drawing;
using static PHLibrary.ExcelExport.ExcelCreatorEPPlus;

namespace PHLibrary.ExcelExport
{
    public class SummaryTableCreator 
    {

        ExcelWorksheet worksheet;
        int bottomMargin ;
        public SummaryTableCreator(ExcelWorksheet worksheet, int bottomMargin = 1)
        {
            this.worksheet = worksheet;
            this.bottomMargin = bottomMargin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summaryData">顶部表格高度</param>
        /// <returns></returns>
        public int Create(IList<IList<string>> summaryData)
        {
            int rowIndex = 0;
            foreach (var list in summaryData)
            {
                
                int colIndex = 0;
                foreach (var cellData in list)
                {

                    worksheet.Cells[rowIndex + 1, colIndex + 1].Value = cellData;
                    colIndex++;
                }
                rowIndex++;
            }
            return rowIndex+bottomMargin;
        }

    }



    public class ExceHeaderCreatorEPPlus  
    {
        public ColumnTree Tree { get; set; }
        ExcelWorksheet sheet;
        Color? headerColor;
        int startRowIndex;
        public ExceHeaderCreatorEPPlus(ColumnTree tree, ExcelWorksheet sheet, Color? headerColor = null, int startRowIndex = 0)
        {
            this.sheet = sheet;
            this.headerColor = headerColor;
            this.Tree = tree;
            this.startRowIndex = startRowIndex;
        }


        public int CreateHeader(out IList<string> formats)
        {
            int width = 1;
            int height = 1;
            try
            {
                height = Tree.Roots.Max(x => x.MaxDepth);
                width = Tree.Roots.Sum(x => x.CalculateLeaesCount());
            }
            catch (System.InvalidOperationException ex)
            {
                //空列
            }
            var allRetangles = Tree.CalculateRetangles(startRowIndex);
            formats = Tree.Roots.SelectMany(x => x.CalculateLeaves()).Select(x => x.Format).ToList();
            //创建原子单元格
            height+=startRowIndex;
            //columns
            for (int column = 0; column < width; column++)
            {
                //rows
                for (int row = startRowIndex; row < height; row++)
                {
                    var cell = sheet.Cells[row + 1, column + 1];


                    var retangle = allRetangles.FirstOrDefault(x => x.RetanglePosition.X == column && x.RetanglePosition.Y == row);
                    if (retangle != null)
                    {


                        cell.Value = retangle.Title;
                        SetDataValications(sheet, column + 1, retangle.Title, retangle.Candidates);

                        if (retangle.ColumnWidth.HasValue)
                        {
                            sheet.Column(column + 1).Width = retangle.ColumnWidth.Value;
                        }
                        else
                        {
                            cell.AutoFitColumns();
                        }

                    }

                }

            }

            //  计算合并
            foreach (var retangle in allRetangles)
            {

                var mergedCellDefine = new MergedCell(retangle);
                var mergedCell = sheet.Cells[
                     mergedCellDefine.StartRow, mergedCellDefine.StartColumn,
                     mergedCellDefine.EndRow, mergedCellDefine.EndColumn];
                mergedCell.Merge = true;
                mergedCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                mergedCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                mergedCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                //mergedCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (headerColor.HasValue)
                {

                    mergedCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    mergedCell.Style.Fill.BackgroundColor.SetColor(headerColor.Value);
                }
                mergedCell.Value = retangle.Title;

            }
            return height;
        }


        public class MergedCell
        {
            public MergedCell(int startRow, int endRow, int startColumn, int endColumn)
            {
                StartRow = startRow;
                EndRow = endRow;
                StartColumn = startColumn;
                EndColumn = endColumn;
            }
            public MergedCell(MergedCellRetangle retangle)
            {
                StartRow = retangle.RetanglePosition.Y + 1;
                EndRow = retangle.RetanglePosition.Y + retangle.RetangleSize.Height;
                StartColumn = retangle.RetanglePosition.X + 1;
                EndColumn = retangle.RetanglePosition.X + retangle.RetangleSize.Width;
            }
            public int StartRow { get; protected set; }
            public int EndRow { get; protected set; }
            public int StartColumn { get; protected set; }
            public int EndColumn { get; protected set; }

        }

        private void SetDataValications(ExcelWorksheet sheet, int columnIndex, string title, IList<string> data)
        {

            var cellsExceptHeader = ExcelCellBase.GetAddress(2, columnIndex, ExcelPackage.MaxRows, columnIndex);
            /*
                var val = ws.DataValidations.AddListValidation("A1");
    val.Formula.ExcelFormula = "B1:B5";
             */
            if (data?.Count > 0)
            {
                //数据引用sheet
                var refSheet = sheet.Workbook.Worksheets.Add(title);
                refSheet.Cells.LoadFromCollection(data);
                refSheet.Hidden = eWorkSheetHidden.Hidden;
                var dataValidationList = sheet.DataValidations.AddListValidation(cellsExceptHeader);
                dataValidationList.Formula.ExcelFormula = $"{title}!$A:$A";
                //for (int i = 0; i < columnWithDdlData.Data.Count; i++)
                //{
                //    unitmeasure.Formula.Values.Add(columnWithDdlData.Data[i]);
                //}
            }
        }
    }
}
