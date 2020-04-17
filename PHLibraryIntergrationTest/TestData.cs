using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibraryIntergrationTest
{
    public class TestData
    {
        public static ColumnTreeNode TestNode
        {
            get
            {
                return new ColumnTreeNode
                {
                   
                    Children = new List<ColumnTreeNode> {
                    new ColumnTreeNode{
                            CanSpanRows=true,
                            Children=new List<ColumnTreeNode>{
                                new ColumnTreeNode() ,new ColumnTreeNode()
                                }
                            },
                    new ColumnTreeNode{

                            Children=new List<ColumnTreeNode>{
                                new ColumnTreeNode(),
                                new ColumnTreeNode(),
                                new ColumnTreeNode{
                                    Children=new List<ColumnTreeNode>{
                                        new ColumnTreeNode(),
                                        new ColumnTreeNode(),
                                        new ColumnTreeNode(),
                                        new ColumnTreeNode()
                                        } }
                             } }
                }
                };
            }
        }
    }
}
