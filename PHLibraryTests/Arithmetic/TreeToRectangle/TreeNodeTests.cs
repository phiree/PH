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
            var tree = new ColumnTree
            {
                Roots = new List<ColumnTreeNode> {
                    TestNode }
            };
            var retangles = tree.CalculateRetangles(0);
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
            var retangles = node.CalculateRetangles(0, node.MaxDepth);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(retangles));
            Assert.AreEqual(11, retangles.Count);



        }

        private ColumnTreeNode TestNode
        {
            get
            {
                return new ColumnTreeNode
                {
                    Title = "基本信息",
                    Children = new List<ColumnTreeNode> {
                    new ColumnTreeNode{
                        Title="人员信息",
                            Children=new List<ColumnTreeNode>{
                                new ColumnTreeNode{ Title="姓名"} } },
                    new ColumnTreeNode{
                        Title="环境信息",
                            Children=new List<ColumnTreeNode>{
                                new ColumnTreeNode{ Title="气压",},
                                new ColumnTreeNode{  Title="湿度",},
                                new ColumnTreeNode{
                                     Title="污染信息",
                                    Children=new List<ColumnTreeNode>{
                                        new ColumnTreeNode{  Title="pm2.5",},
                                        new ColumnTreeNode{  Title="二氧化硫",},
                                        new ColumnTreeNode{  Title="pm10",},
                                        new ColumnTreeNode{  Title="臭氧",}
                                        } }
                             } }
                }
                };
            }
        }

        [TestMethod()]
        public void CalculateLeaesCountTest()
        {
            var node = TestNode;
            Assert.AreEqual(7, node.CalculateLeaesCount());
            Assert.AreEqual(4, node.Children[1].Children[2].CalculateLeaesCount());
            Assert.AreEqual(6, node.Children[1].CalculateLeaesCount());
        }

        [TestMethod()]
        public void CalculateLeavesTest()
        {
            var node=TestNode;
            var leaves=node.CalculateLeaves();
                Assert.AreEqual(7,leaves.Count);
           
        }
    }
}