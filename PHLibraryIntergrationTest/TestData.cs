using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibraryIntergrationTest
{
    public class TestData
    {
        public static TreeNode TestNode
        {
            get
            {
                return new TreeNode
                {
                   
                    Children = new List<TreeNode> {
                    new TreeNode{
                            CanSpanRows=true,
                            Children=new List<TreeNode>{
                                new TreeNode() ,new TreeNode()
                                }
                            },
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
