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

        Label label1= new Label { AutoSize = true,Dock= DockStyle.Right };
        Panel panel=new Panel { Dock= DockStyle.Fill};
        public ColumnTreeToRetangle()
        {
            InitializeComponent();
           
            Controls.Add(label1);
            Controls.Add(panel);
          
        }
        protected override void OnPaint(PaintEventArgs e)
        {
           
            Draw(panel.CreateGraphics());
        }
        
        
        
        private Random rnd = new Random();
        public void Draw(System.Drawing.Graphics formGraphics)
        {
            var node = TestData.TestNode;
            var node2=TestData.TestNode;
            var tree=new ColumnTree { Roots=new List<PHLibrary.Arithmetic.TreeToRectangle.ColumnTreeNode> { node,node2} };
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
