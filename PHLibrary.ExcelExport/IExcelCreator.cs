using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PHLibrary.ExcelExport
{
    /// <summary>
    /// 导出dataset
    /// </summary>
   public  interface IExcelCreator
    {
        System.IO.Stream Create(DataSet dataset, ColumnTree columnTree, CellStyleSettings cellStyleSettings = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataToExport">使用datatable的列名作为表头</param>
        /// <returns></returns>
        System.IO.Stream Create(System.Data.DataSet dataToExport, CellStyleSettings cellStyleSettings = null);
        System.IO.Stream Create(DataTable dataTable, CellStyleSettings cellStyleSettings = null);
        /// <summary>
        /// excel表头文字的计算顺序：
        /// 1参数里的属性名映射字典
        /// 2 属性的DisplayName Attribute
        /// 3 属性名本身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="propertyNameMaps"></param>
        /// <returns></returns>
         System.IO.Stream Create<T>(IList<T> data, IDictionary<string, string> propertyNameMaps=null, CellStyleSettings cellStyleSettings = null);
         System.IO.Stream Create<T>(IList<T> data, ColumnTree tree, CellStyleSettings cellStyleSettings = null);
        System.IO.Stream Create(DataTable dataTable,ColumnTree columnTree, CellStyleSettings cellStyleSettings = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataToExport"></param>
        /// <param name="headerTrees">表头树形结构定义,可以构建合并单元格的表头. </param>
        /// <returns></returns>
        System.IO.Stream Create(System.Data.DataSet dataToExport,IList<ColumnTree> headerTrees, CellStyleSettings cellStyleSettings = null);
    }
 

}
