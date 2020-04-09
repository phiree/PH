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
                    Text="基本信息",
                    Children = new List<TreeNode> {
                    new TreeNode{
                        Text="人员信息",
                            Children=new List<TreeNode>{
                                new TreeNode{ Text="姓名"} } },
                    new TreeNode{
                        Text="环境信息",
                            Children=new List<TreeNode>{
                                new TreeNode{ Text="气压",},
                                new TreeNode{  Text="湿度",},
                                new TreeNode{
                                     Text="污染信息",
                                    Children=new List<TreeNode>{
                                        new TreeNode{  Text="pm2.5",},
                                        new TreeNode{  Text="二氧化硫",},
                                        new TreeNode{  Text="pm10",},
                                        new TreeNode{  Text="臭氧",}
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