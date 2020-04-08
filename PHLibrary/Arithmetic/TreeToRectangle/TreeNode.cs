using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace PHLibrary.Arithmetic.TreeToRectangle
{
    public class Tree
    {
        public IList<TreeNode> Roots { get; set; }
        /// <summary>
        /// 整棵树占用的矩形
        /// </summary>
        /// <returns></returns>
        public RetangleSize CalculateWholeRetangle()
        {
            var maxDepth =  Roots.Max(n => n.MaxDepth);
            int x = 0;
            foreach (var node in Roots)
            {
                var nodeRetangle = node.CalculateRetangle(x, node.MaxDepth);
                x = x + nodeRetangle.RetangleSize.Width;

            }
            var width = x;
            return new RetangleSize(width, maxDepth);

        }

    }

    public class TreeNode
    {
        public IList<TreeNode> Children { get; set; }
        public int? RowSpan { get; set; } = 1;

        int currentRetangleIndex = 1;
        
        /// <summary>
        /// 该节点占用的矩形
        /// </summary>
        /// <returns></returns>
        public Retangle CalculateRetangle(int initialX, int bigRetangleHeight)
        {
            var nodeMaxDepth = AllPathDepths[currentRetangleIndex];

            var retangleSize = new RetangleSize(1, 1);
            if (Children == null || Children.Count == 0)
            {
                return new Retangle(new RetanglePosition(initialX, 0), new RetangleSize(1, bigRetangleHeight));
            }
            foreach (var c in Children)
            {
                retangleSize.InitHeight();
                _CalculateRetangle(c, retangleSize, bigRetangleHeight);
            }
            return new Retangle(null, retangleSize);
        }


        public void _CalculateRetangle(TreeNode columnDefine, RetangleSize retangleSize, int bigRetangleHeight)
        {
            int heightToExtent = 1;
            if (MaxDepth < bigRetangleHeight)
            {
                if (RowSpan != null)
                {
                    heightToExtent = RowSpan.Value;
                }
                else
                {
                    heightToExtent = bigRetangleHeight - MaxDepth + 1;
                }
            }

            retangleSize.ExtentHeight(heightToExtent);
            if (columnDefine.Children == null || columnDefine.Children.Count == 0)
            {
                retangleSize.ExtentWidth(1);
                currentRetangleIndex += 1;
                return;
            }
            foreach (var col in columnDefine.Children)
            {
                _CalculateRetangle(col, retangleSize, bigRetangleHeight);
            }
        }
        /// <summary>
        /// 获取最大路径长度
        /// </summary>
        public int MaxDepth { get  { 
                if(AllPathDepths.Count==0)
                { 
                    CalculateAllPathDepths();
                    }
                return AllPathDepths.Max(x=>x.Value);} }
        /// <summary>
        /// 计算出所有路径的长度
        /// </summary>
        /// <returns></returns>
        public void CalculateAllPathDepths()
        {

            int depth = 1;

            if (Children == null)
            {
                AllPathDepths.Add(currentPathIndex, depth);
                return;
            }

            foreach (var c in Children)
            {
                _CalculateDept(c, ref depth);
            }
            
        }
        int currentPathIndex = 1;
        public IDictionary<int, int> AllPathDepths { get;protected set;} =new Dictionary<int,int>();
        //IList<int> allDepts = new List<int>();
        public void _CalculateDept(TreeNode node, ref int depth)
        {
            depth += 1;
            if (node.Children == null)
            {
                depth -= 1;
                // allDepts.Add( depth);
                AllPathDepths.Add(currentPathIndex, depth);
                currentPathIndex += 1;
                return;
            }
            foreach (var c in node.Children)
            {
                _CalculateDept(c, ref depth);
            }
        }
    }
}
