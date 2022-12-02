using System.Collections.Generic;

namespace PHLibrary.ExcelExportExcelCreator
{
    public class TwoDimensionalDefine
    {
        public TwoDimensionalX TwoDimensionalX { get; set; }

        public string YName { get; set; }

        public TwoDimensionalDefine(string xName, string xGuid, string yName)
        {
            TwoDimensionalX = new TwoDimensionalX(xName, xGuid);

            YName = yName;
        }
       

        public string[] BuildOrderBy() { 
            var orderBy=new List<string>();
            orderBy.Add(TwoDimensionalX.Name);
            if (TwoDimensionalX.Guid != null) { 
                orderBy.Add(TwoDimensionalX.Guid);
                }
            return orderBy.ToArray();
            }
        public bool Contains(string columnName)
        {
            return TwoDimensionalX.Name == columnName || TwoDimensionalX.Guid == columnName || YName == columnName;
        }
    }
}
