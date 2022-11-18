using System.Collections.Generic;
using System.Data;
 

namespace PHLibrary.ExcelExportExcelCreator
{
    public interface IDataTableConverter<T>
    {
        /// <summary>
        /// 将类型(包含dynamic)转换为 datatable
        /// </summary>
        /// <param name="data">数据集合,用于提取属性名称,数据</param>
        /// <param name="propertyNameMaps">属性名称对应关系.</param>
        /// <returns></returns>
          DataTable Convert(IList<T> data, SortSize sortSize, string amountFormat, IList<string> propertiesToDisplay);
    }
}