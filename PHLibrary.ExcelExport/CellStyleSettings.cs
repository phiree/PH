using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    /// 单元格样式设置
    /// </summary>
    public class CellStyleSettings
    {
        public OfficeOpenXml.Style.ExcelBorderStyle BorderStyle { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.None;
    }
}
