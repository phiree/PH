using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace PHLibrary.ExcelExportExcelCreator
{
    public class DataTableWithExtensions
    {
        public DataTable DataTable { get; set; }
        public Dictionary<int, Image> Images { get; set; }

    }
}
