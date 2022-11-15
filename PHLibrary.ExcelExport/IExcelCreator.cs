using PHLibrary.Arithmetic.TreeToRectangle;
using PHLibrary.ExcelExportExcelCreator;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    /// 导出dataset
    /// </summary>
   public  interface IExcelCreator
    {
          Stream Create<T>(IList<T> data, SortSize sortSize, IList<IList<string>> summaryData, int summaryTableBottomMargin ,CellStyleSettings cellStyleSettings, string amountFormat);
    }
 

}
