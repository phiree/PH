 
using PHLibrary.Arithmetic.TreeToRectangle;
using System.Collections.Generic;

namespace PHLibrary.ExcelExport
{
    public interface IExcelHeaderCreater
    {
          ColumnTree Tree { get;   set; }
        /// <summary>
        /// 创建表头
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns>表头的高度</returns>
        /// <param name="formats">列的格式</param>
        int CreateHeader( out IList<string> formats);
    }
     
}