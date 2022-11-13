using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Arithmetic.TreeToRectangle;
using PHLibrary.ExcelExport;
using PHLibrary.ExcelExportExcelCreator;
using PHLibrary.Reflection;
using PHLibrary.Reflection.ArrayValuesToInstance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using static PHLibrary.ExcelExport.ExcelCreatorEPPlus;

namespace PHLibrary.ExcelExport.Tests
{
    [TestClass()]
    public class ExcelCreatorEPPlusTests
    {
        public class TestItem
        {
            [Column( "姓名")]
            public string Name { get; set; }
            [Column("年龄")]
            public int Age { get; set; }
            [ImageColumn]
            [Column("图片")]
            public string Picture { get;set;}
        }
        
        [TestMethod()]
        public void CreateForImages()
        {
           
            var list = new List<TestItem>() { 
                new TestItem{ Age=1, Name="name",Picture=
                "https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135"  }
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list,null,null,1, null);
            using (var file = new FileStream("CreateForImages" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }

        public class Order1103
        {
            [Column("品名")]
            [PropertyOrder(1)]
            public string Name { get; set; }
            [Column("货号")]
            public string Code { get; set; }
            [Column("图片")]
            [ImageColumn]
            public string Picture { get; set; }
            [Column("颜色")]
            public string Color { get; set; }
            [TwoDimensional(true)]
            public string Size { get; set; }
            [TwoDimensionalGuid]
            public Guid SizeGuid { get;set;}
            [TwoDimensional]
            public int Amount { get; set; }

        }
        [TestMethod()]
        public void CreateForImagesAndTwoDimetionalData()
        {
     
            Guid guidM=Guid.NewGuid();
            Guid guidL= Guid.NewGuid();
            Guid guidXL= Guid.NewGuid();
            Guid guidXXL= Guid.NewGuid();
            Guid guidXXXL= Guid.NewGuid();
            var list = new List<Order1103>() { 
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM ,  Amount=1 },
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                    Size="L",SizeGuid=guidL,  Amount=2 },
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                    Size="XL",SizeGuid=guidXL,  Amount=3 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM,  Amount=4 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="L",SizeGuid=guidL ,  Amount=5 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                     Size="XXL",SizeGuid=guidXXL,  Amount=6 },
                new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                     Size="XXXL",SizeGuid=guidXXXL,  Amount=7 },
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list,Sort,null,1, null);
            using (var file = new FileStream("CreateForImagesAndTwoDimetional" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        private IList<string> Sort(IList<TwoDimensionalX> columns)
        {
            return columns.OrderBy(x=>x.Guid).Select(x=>x.Name).Distinct().ToList();
            
        }
        
        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        [TestMethod()]
        public void CreateForSummaryTable()
        {

            var list = new List<TestItem>() {
                new TestItem{ Age=1, Name="name",Picture=
                "https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135"  }
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list, null,
                new List<IList<string>> {
                    new List<string>{"客户","张三" }
                    ,new List<string>{ "电话","13300000000"} 
                    ,new List<string>{ "地址","地址111111111" }
                    ,new List<string>{ "总金额","1230"} 
                    ,new List<string>{ "备注","已付款"} 
                    }
                ,4
                , null);
            using (var file = new FileStream("CreateForImages" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
    }
}