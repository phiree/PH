using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Arithmetic.TreeToRectangle
{
    public class Converter
    {
        IList<TreeNode> nodes;
        public Converter(IList<TreeNode> nodes) { 
            this.nodes=nodes;
            }
        public IList<Retangle> Convert()
        {
            throw new Exception();
            var retangles=new List<Retangle>();
 
            int currentDepth=0;//树枝当前层级
            int currentWidthInCurrentDepth=0; //层级的当前宽度
            foreach (var treenode in nodes)
            {
                currentWidthInCurrentDepth=0;
                

                
            }
        }
       
    }
}
