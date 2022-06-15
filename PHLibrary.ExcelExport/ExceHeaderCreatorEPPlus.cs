using System.Collections.Generic;
using System.Linq;
using PHLibrary.Arithmetic.TreeToRectangle;
using OfficeOpenXml;
using System.Drawing;

namespace PHLibrary.ExcelExport
{
    public class ExceHeaderCreatorEPPlus : IExcelHeaderCreater
    {
        public ColumnTree Tree { get;set;}
        ExcelWorksheet sheet;
        Color? headerColor;
        public ExceHeaderCreatorEPPlus(ColumnTree tree, ExcelWorksheet sheet,Color? headerColor=null )
        {
            this.sheet=sheet;
            this.headerColor =  headerColor;
            this.Tree = tree;

        }

        
        public int CreateHeader( out IList<string> formats)
        {
            int width = Tree.Roots.Sum(x => x.CalculateLeaesCount());
            int height = Tree.Roots.Max(x => x.MaxDepth);
            var allRetangles = Tree.CalculateRetangles();
            formats = Tree.Roots.SelectMany(x => x.CalculateLeaves()).Select(x => x.Format).ToList();
            //创建原子单元格

            for (int i = 0; i < height; i++)
            {
                

                for (int j = 0; j < width; j++)
                {
                    var cell = sheet.Cells[i+1,j+1];
                    
                     
                    var retangle = allRetangles.FirstOrDefault(x => x.RetanglePosition.X == j && x.RetanglePosition.Y == i);
                    if (retangle != null)
                    {
                       
                        
                        cell.Value=retangle.Title;
                        cell.AutoFitColumns();
                    }

                }

            }
            
            //  计算合并
            foreach (var retangle in allRetangles)
            {
                
                var mergedCellDefine = new MergedCell(retangle);
               var mergedCell= sheet.Cells[
                    mergedCellDefine.StartRow,  mergedCellDefine.StartColumn, 
                    mergedCellDefine.EndRow, mergedCellDefine.EndColumn];
                mergedCell.Merge=true;
                mergedCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                mergedCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
               
                mergedCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                mergedCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if(headerColor.HasValue)
                { 
                mergedCell.Style.Fill.BackgroundColor.SetColor(headerColor.Value);
                }
                mergedCell.Value=retangle.Title;

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
                StartRow = retangle.RetanglePosition.Y+1;
                EndRow = retangle.RetanglePosition.Y + retangle.RetangleSize.Height ;
                StartColumn = retangle.RetanglePosition.X+1;
                EndColumn = retangle.RetanglePosition.X + retangle.RetangleSize.Width ;
            }
            public int StartRow { get; protected set; }
            public int EndRow { get; protected set; }
            public int StartColumn { get; protected set; }
            public int EndColumn { get; protected set; }

        }
    }
}
