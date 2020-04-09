using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

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
            var retangles = tree.CalculateRetangles();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(retangles));
           

              Assert.AreEqual(11, retangles.Count);
        }
        [TestMethod()]
        public void CalculateMaxDeptTest()
        {
            var node = TestNode;
            Assert.AreEqual(4, node.MaxDepth);

            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(node.AllPathDepths));

        }

        [TestMethod()]
        public void CalculateRetangleTest()
        {
            var node = TestNode;
         var retangles=  node.CalculateRetangles(0, node.MaxDepth);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(retangles));
            Assert.AreEqual(11,retangles.Count);
           


        }

        private TreeNode TestNode
        {
            get
            {
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

        [TestMethod()]
        public void CalculateLeaesCountTest()
        {
           var node=TestNode;
         Assert.AreEqual(7, node.CalculateLeaesCount());
            Assert.AreEqual(4,node.Children[1].Children[2].CalculateLeaesCount());
            Assert.AreEqual(6, node.Children[1]. CalculateLeaesCount());
        }
    }
}