using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Arithmetic.TreeToRectangle
{
    /// <summary>
    /// 合并单元格对应的长方形(坐标,长宽)
    /// </summary>
   public class MergedCellRetangle
    {
        public MergedCellRetangle(RetanglePosition retanglePosition, RetangleSize retangleSize, string text)
            :this(retanglePosition,retangleSize,text,string.Empty,0)
        {
           
        }
        public MergedCellRetangle(RetanglePosition retanglePosition, RetangleSize retangleSize, string text,string format)
           : this(retanglePosition, retangleSize, text,format, 0)
        {

        }
        public MergedCellRetangle(RetanglePosition retanglePosition, RetangleSize retangleSize,string text,string format,int columnWidth)
        {
            RetanglePosition = retanglePosition;
            RetangleSize = retangleSize;
            this.Title=text;
            Format=format;
            this.ColumnWidth= columnWidth;
        }
        /// <summary>
        /// 单元格标题
        /// </summary>
        public string Title { get;protected set;}
        /// <summary>
        /// 宽度
        /// </summary>
        public int ColumnWidth { get;set;}
        /// <summary>
        /// excel数值格式化表达式
        /// </summary>
        public string Format { get;protected set;}

        public RetanglePosition RetanglePosition { get; protected set; }
        public RetangleSize RetangleSize { get; protected set; }
        public override string ToString()
        {
            return $"{RetanglePosition.X},{RetanglePosition.Y},{RetangleSize.Width},{RetangleSize.Height},{Title}";
        }

    }
    public class RetanglePosition {
        public RetanglePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; protected set; }
        public int Y { get; protected set; }
    }
    public class RetangleSize
    {
        public RetangleSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get;protected set;}
        public int Height { get; protected set; }

        public void ExtentWidth(int with) { this.Width+=with;}
        public void ExtentHeight(int height) { this.Height += height; }
        /// <summary>
        /// 重置高度
        /// </summary>
        public void InitHeight() {this.Height=0; }
    }
}
