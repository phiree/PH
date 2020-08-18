using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace PHLibrary.Arithmetic.TreeToRectangle
{
    /// <summary>
    /// 树形列定义
    /// </summary>
    public class ColumnTree
    {
        public IList<ColumnTreeNode> Roots { get; set; }
        /// <summary>
        /// 根据树形结构 创建对应的矩形
        ///     目前应用: 创建合并单元格的表头
        /// </summary>
        /// <returns></returns>
        /// 
       
        public IList<MergedCellRetangle> CalculateRetangles()
        {
            var maxDepth = Roots.Max(n => n.MaxDepth);

            var allRetangles = new List<MergedCellRetangle>();

            var totalWidth = 0;
            foreach (var node in Roots)
            {
                allRetangles.AddRange(node.CalculateRetangles(totalWidth, maxDepth));
                totalWidth = totalWidth + node.CalculateLeaesCount();
            }
            return allRetangles;
        }
    }
}
