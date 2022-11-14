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
        public bool Contains(string columnName)
        {
            return TwoDimensionalX.Name == columnName || TwoDimensionalX.Guid == columnName || YName == columnName;
        }
    }
}
