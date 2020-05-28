using PHLibrary.Arithmetic.TreeToRectangle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PHLibraryIntergrationTest
{
    public partial class ColumnTreeToRetangle : Form
    {

        Label label1= new Label { AutoSize = true,Dock= DockStyle.Left };
        Panel panel=new Panel { Dock= DockStyle.Fill};
      Panel panel2 = new Panel { Dock = DockStyle.Right };
        public ColumnTreeToRetangle()
        {
            InitializeComponent();
           
            Controls.Add(label1);
            Controls.Add(panel);
          Controls.Add(panel2);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var node = TestData.TestNode;
            var node2 = TestData.TestNode;
            var tree = new ColumnTree { Roots = new List<PHLibrary.Arithmetic.TreeToRectangle.ColumnTreeNode> { node, node2 } };

             Draw(tree,panel.CreateGraphics());

            var simpleTree=new ColumnTree { Roots=new List<ColumnTreeNode>{ 
                //root1
                new ColumnTreeNode{ CanSpanRows=true  },
                //root2
                new ColumnTreeNode{  Children=new List<ColumnTreeNode>{ new ColumnTreeNode()} }
                } };
            Draw(simpleTree, panel2.CreateGraphics());
        }
        
        
        
        private Random rnd = new Random();
        public void Draw(ColumnTree tree, System.Drawing.Graphics formGraphics)
        {
            // var retangles = node.CalculateRetangles(0, node.MaxDepth);
           var  retangles=tree.CalculateRetangles();
            
                  foreach (var c in retangles)
            {
                label1.Text+=c.ToString()+Environment.NewLine;
                System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush
                    (Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                formGraphics.FillRectangle(myBrush, new Rectangle(
                    c.RetanglePosition.X * 20,
                    c.RetanglePosition.Y * 20,
                    c.RetangleSize.Width * 20,
                    c.RetangleSize.Height * 20));
                myBrush.Dispose();

            }
           
            
        }

    }
}
