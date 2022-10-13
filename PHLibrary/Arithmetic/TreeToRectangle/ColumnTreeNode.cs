using System.Collections.Generic;
using System.Linq;
namespace PHLibrary.Arithmetic.TreeToRectangle
{
    /// <summary>
    /// 树形列定义的节点
    /// </summary>
    public class  ColumnTreeNode
    {

        public string Title { get; set; }
        /// <summary>
        /// 该列候选值
        /// </summary>
        public IList<string> Candidates { get;set;}
        public string Format { get; set; }
        public int? ColumnWidth { get;set;}
        public IList<ColumnTreeNode> Children { get; set; }

        IList<MergedCellRetangle> retangles = new List<MergedCellRetangle>();
        public bool CanSpanRows { get; set; }


        int bigRetangleHeight;

        /// <summary>
        /// 计算该几点占用的矩形
        /// </summary>
        /// <returns></returns>
        public IList<MergedCellRetangle> CalculateRetangles(int initialX, int bigRetangleHeight)
        {
            this.bigRetangleHeight = bigRetangleHeight;

            int height = GetHeight(this);
            int width = CalculateLeaesCount();
            int initialY = 0;
            retangles.Add(new MergedCellRetangle(new RetanglePosition(initialX, initialY)
                , new RetangleSize(width, height), Title, Format,ColumnWidth,Candidates));
            if (Children != null)
            {
                initialY += height;
                foreach (var c in Children)
                {

                    _CalculateRetangle(c, initialX, initialY);
                    initialX += c.CalculateLeaesCount();

                }

            }
            return retangles;
        }


        int currentRetangleIndex = 1;

        private int GetHeight(ColumnTreeNode node)
        {

            var maxDepth = AllPathDepths[currentRetangleIndex];
            int height = 1;
            if (maxDepth < bigRetangleHeight)
            {
                if (node.CanSpanRows)
                {

                    height = bigRetangleHeight - maxDepth + 1;
                }
            }
            return height;

        }
        public void _CalculateRetangle(ColumnTreeNode node, int initialX, int initialY)
        {
            int height = GetHeight(node);
            int width = node.CalculateLeaesCount();
            int currentX = initialX;
            retangles.Add(new MergedCellRetangle(new RetanglePosition(initialX, initialY),
                new RetangleSize(width, height)
                , node.Title, node.Format
                ));
            if (node.Children != null)
            {
                initialY += height;
                foreach (var c in node.Children)
                {

                    _CalculateRetangle(c, currentX, initialY);
                    currentX += c.CalculateLeaesCount();
                }

            }
        }

        public IList<ColumnTreeNode> CalculateLeaves()
        {
            var leaves = new List<ColumnTreeNode>();
            if (Children == null)
            {
                leaves.Add(this);
            }
            else
            {
                foreach (var c in Children)
                {
                    _CalculateLeaves(c, leaves);
                }
            }
            return leaves;
        }
        public void _CalculateLeaves(ColumnTreeNode treeNode, IList<ColumnTreeNode> leaves)
        {
            if (treeNode.Children == null) { leaves.Add(treeNode); }
            else
            {
                foreach (var c in treeNode.Children)
                {

                    _CalculateLeaves(c, leaves);
                }
            }
        }
   
    /// <summary>
    /// 树叶数量
    /// </summary>
    /// <returns></returns>
    public int CalculateLeaesCount()
    {
            
            var leaves=CalculateLeaves();
            
        int count = 0;
        if (Children == null) { return count + 1; }
        else
        {
            foreach (var c in Children)
            {
                _CalculateLeaesCount(c, ref count);
            }
        }
        return count;
    }
    public void _CalculateLeaesCount(ColumnTreeNode child, ref int count)
    {
        if (child.Children == null) { count += 1; }
        else
        {
            foreach (var cc in child.Children) { _CalculateLeaesCount(cc, ref count); }
        }

    }
    /// <summary>
    /// 获取最大路径长度
    /// </summary>
    public int MaxDepth
    {
        get
        {
            if (AllPathDepths.Count == 0)
            {
                CalculateAllPathDepths();
            }
            return AllPathDepths.Max(x => x.Value);
        }
    }
    /// <summary>
    /// 计算出所有路径的长度
    /// </summary>
    /// <returns></returns>
    public void CalculateAllPathDepths()
    {
        int initDepth = 0;
        int depth = initDepth + 1;

        if (Children == null)
        {
            AllPathDepths.Add(currentPathIndex, depth);
            return;
        }

        foreach (var c in Children)
        {
            _CalculateDept(c, depth + 1);
        }

    }
    int currentPathIndex = 1;



    public IDictionary<int, int> AllPathDepths { get; protected set; } = new Dictionary<int, int>();
    //IList<int> allDepts = new List<int>();
    public void _CalculateDept(ColumnTreeNode node, int depth)
    {

        if (node.Children == null)
        {
            AllPathDepths.Add(currentPathIndex, depth);
            currentPathIndex += 1;
            return;
        }
        foreach (var c in node.Children)
        {
            _CalculateDept(c, depth + 1);
        }
    }
}
}
