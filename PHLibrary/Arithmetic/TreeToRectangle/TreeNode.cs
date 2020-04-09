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
        public IList<Retangle> CalculateWholeRetangle()
        {
            var maxDepth = Roots.Max(n => n.MaxDepth);
            var allRetangles = new List<Retangle>();

            int x = 0;
            foreach (var node in Roots)
            {

                allRetangles.AddRange(node.CalculateRetangles(x, maxDepth));
                x = x + node.CalculateLeaesCount();

            }
            var width = x;
            return allRetangles;

        }

    }

    public class TreeNode
    {
        public string Text { get;set;}
        public IList<TreeNode> Children { get; set; }

        IList<Retangle> retangles = new List<Retangle>();
        public bool CanSpanRows { get;set;}


        int bigRetangleHeight;
         
        /// <summary>
        /// 该节点占用的矩形
        /// </summary>
        /// <returns></returns>
        public IList<Retangle> CalculateRetangles(int initialX, int bigRetangleHeight)
        {
            this.bigRetangleHeight=bigRetangleHeight;
            
            int height=GetHeight(this);
            int width=CalculateLeaesCount();
            int initialY=0;
            retangles.Add(new Retangle(new RetanglePosition(initialX,initialY), new RetangleSize(width,height )));
            if (Children != null)
            {
                initialY += height;
                foreach (var c in Children)
                {
                  
                    _CalculateRetangle(c,initialX,initialY);
                    initialX+=c.CalculateLeaesCount();
                    
                }
              
            }
            return retangles;
        }

         
        int currentRetangleIndex = 1;
        
        private int GetHeight(TreeNode node)
        {
            
            var maxDepth = AllPathDepths[currentRetangleIndex];
            int height = 1;
            if ( maxDepth < bigRetangleHeight)
            {
                if (node.CanSpanRows)
                {
                 
                    height = bigRetangleHeight - maxDepth + 1;
                }
            }
            return height;

        }
        public void _CalculateRetangle(TreeNode node,int initialX,int initialY)
        {
            int height = GetHeight(node);
            int width = node.CalculateLeaesCount();
             int currentX=initialX;
            retangles.Add(new Retangle(new RetanglePosition(initialX, initialY), new RetangleSize(width, height)));
            if (node.Children != null)
            {
                initialY += height;
                foreach (var c in node.Children)
                {
                   
                    _CalculateRetangle(c, currentX, initialY);
                    currentX+=c.CalculateLeaesCount();
                }
              
            }
        }
        /// <summary>
        /// 树叶数量
        /// </summary>
        /// <returns></returns>
        public int CalculateLeaesCount()
        {
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

        public void _CalculateLeaesCount(TreeNode child, ref int count)
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
            int initDepth=0;
            int depth = initDepth+1;

            if (Children == null)
            {
                AllPathDepths.Add(currentPathIndex, depth);
                return;
            }

            foreach (var c in Children)
            { 
                _CalculateDept(c,  depth+1);
            }

        }
        int currentPathIndex = 1;
        public IDictionary<int, int> AllPathDepths { get; protected set; } = new Dictionary<int, int>();
        //IList<int> allDepts = new List<int>();
        public void _CalculateDept(TreeNode node,   int depth)
        {
          
            if (node.Children == null)
            {
                AllPathDepths.Add(currentPathIndex, depth);
                currentPathIndex += 1;
                return;
            }
            foreach (var c in node.Children)
            {
                _CalculateDept(c, depth+1);
            }
        }
    }
}
