using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace PHLibrary.Arithmetic.TreeToRectangle.Tests
{
    [TestClass()]
    public class TreeNodeTests
    {
        [TestMethod()]
        public void TreeNodeTest()
        {
            var tree = new Tree
            {
                Roots = new List<TreeNode> {
                    TestNode }
            };
            var retangle=tree.CalculateWholeRetangle();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(retangle));
            Assert.AreEqual(5, retangle.Width);
            
            //    Assert.AreEqual(3, tree.CalculateWholeRetangle().Height);
        }
        [TestMethod()]
        public void CalculateMaxDeptTest()
        {
            var node=TestNode;
            Assert.AreEqual(4, node.MaxDepth);

            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(node.AllPathDepths));
           
        }

        [TestMethod()]
        public void CalculateRetangleTest()
        {var node=TestNode;
          var retangle= node.CalculateRetangle(0, node.MaxDepth);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(retangle));
          

        }

        private TreeNode TestNode {
            get {
                return new TreeNode
                {
                    Children = new List<TreeNode> {
                    new TreeNode{   
                            Children=new List<TreeNode>{ 
                                new TreeNode()} },
                    new TreeNode{
                            Children=new List<TreeNode>{
                                new TreeNode(),
                            
                                new TreeNode(),
                                new TreeNode{
                                Children=new List<TreeNode>{
                                    new TreeNode(),
                                    new TreeNode(), 
                                    new TreeNode(), 
                                    new TreeNode()
                                    } }
                             } }
                }
                };
            }
            }
    }
}