using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Arithmetic.TreeToRectangle;
using PHLibrary.ExcelExport;
using PHLibrary.ExcelExportExcelCreator;
using PHLibrary.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public void CreateForEmptyList()
        {
            var list = new List<TestItem>();
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForEmptyList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

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
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForImages" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }

        public class Order1103
        {
            [Column("品名")]

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
            [TwoDimensional]
            public int Amount { get; set; }

        }
        [TestMethod()]
        public void CreateForImagesAndTwoDimetionalData()
        {
     
            var list = new List<Order1103>() { 
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                Size="M", Amount=1 },
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                Size="L", Amount=2 },
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                Size="XL", Amount=3 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                Size="M", Amount=4 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                Size="L", Amount=5 },
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135", 
                Size="XXL", Amount=6 },
                new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                Size="XXXL", Amount=7 },
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list,sortSize:Sort);
            using (var file = new FileStream("CreateForImagesAndTwoDimetional" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        private IList<string> Sort(IList<string> columns)
        {
            return new List<string> { "L", "XL", "M", "XXXL","XXL" };
        }
        [TestMethod()]
        public void CreateForNotEmptyDynamicList()
        {
            var list = new List<dynamic> { new { Age = 1, Name = "name1" }, new { Age = 2, Name = "name2" } };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForNotEmptyDynamicList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        [TestMethod()]
        public void CreateForEmptyDynamicList()
        {
            Assert.IsTrue(true);
            return;
            var list = new List<dynamic> { };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForEmptyDynamicList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }

        [TestMethod()]
        public void CreateForWithFormat()
        {
            var list = new List<dynamic> {
                new { Price = 1, Name = "name1" }
            , new { Price = 10, Name = "name2" }
             , new { Price = 101, Name = "name3" }
              , new { Price = 1000, Name = "name4" },
                 new { Price = 10000, Name = "name5" }
            , new { Price = 100000, Name = "name6" }

              };

            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            ColumnTree tree = new ColumnTree
            {
                Roots = new List<ColumnTreeNode> {
                      new ColumnTreeNode{ Title="价格1", Format="¥#,##0.00;¥-#,##0.00" },
                      new ColumnTreeNode{ Title="名称1" }
                     }
            };
            var stream = excelCreator.Create(list, tree, new CellStyleSettings { HeaderBackgroundColor = System.Drawing.Color.AliceBlue, BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Dotted, HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left });
            using (var file = new FileStream("CreateForWithFormat" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }



        [TestMethod()]
        public void CreateForWithWidth()
        {
            var list = new List<dynamic> {
                new { Price = 1, Name = "name1" }
            , new { Price = 10, Name = "name2" }
             , new { Price = 101, Name = "name3" }
              , new { Price = 1000, Name = "name4" },
                 new { Price = 10000, Name = "name5" }
            , new { Price = 987263541.23, Name = "name6" }

              };

            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            ColumnTree tree = new ColumnTree
            {
                Roots = new List<ColumnTreeNode> {
                      new ColumnTreeNode{ Title="价格1" },
                      new ColumnTreeNode{ Title="名称1", ColumnWidth=100}
                     }
            };
            var stream = excelCreator.Create(list, tree, new CellStyleSettings { HeaderBackgroundColor = System.Drawing.Color.AliceBlue, BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Dotted, HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left });
            using (var file = new FileStream("CreateForWithWidth" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }



        [TestMethod()]
        public void CreateForNotEmptyList()
        {
            var list = new List<TestItem> { new TestItem { Age = 1, Name = "name1" }, new TestItem { Age = 2, Name = "name2" } };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list);
            using (var file = new FileStream("CreateForNotEmptyList" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        [TestMethod()]
        public void CreateFromDataSetTest()
        {
            ExcelCreatorEPPlus excelCreator
                = new ExcelCreatorEPPlus();
            Stopwatch watch = Stopwatch.StartNew();

            var stream = excelCreator.Create( CreateDemoDataSet(2, 130, 100));

            using (var file = new FileStream("createdexcel_from_dataset" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
            Console.WriteLine("单元格不画框用时:" + watch.Elapsed);
            watch.Restart();

            var stream2 = excelCreator.Create(CreateDemoDataSet(2, 130, 100), new CellStyleSettings { BorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin });


            using (var file = new FileStream("createdexcel_from_dataset_with_border_" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream2, file);

            }
            Console.WriteLine("  单元格画框用时:" + watch.Elapsed);




        }

        /// <summary>
        /// 原因: 数智沙盘新栅格2020-12月数据5w条 需要6.4minuts, why?
        /// </summary>
        [TestMethod()]
        public void ExportPerformanceTest()
        {
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var config = FluentExcel.Excel.Setting.For<数值沙盘新栅格报表>();

            Stream stream = excelCreator.Create(CreateDemoDataSet(2, 130, 100));
            FluentExcel.Excel.Load<数值沙盘新栅格报表>(stream, 2);
        }
        public class 数值沙盘新栅格报表
        {

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
        private DataSet CreateDemoDataSet(int tableAmount, int columnAmount, int rowAmount)
        {
            DataSet dataSet = new DataSet();
            for (int i = 0; i < tableAmount; i++)
            {

                var table = new DataTable("table_" + i);

                for (int colIndex = 0; colIndex < columnAmount; colIndex++)
                {
                    string title = "col_" + colIndex;
                    table.Columns.Add(new DataColumn(title));

                }
                for (int r = 0; r < rowAmount; r++)
                {

                    var row = table.NewRow();
                    for (int col = 0; col < columnAmount; col++)
                    {
                        row[col] = $"12.312345";
                    }
                    table.Rows.Add(row);
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;

        }

      
        [TestMethod()]
        public void CreateWithCandidates ()
        {
            //var list = new List<dynamic> {
            //    new { Price = 1, Name = "name1" }
            //, new { Price = 10, Name = "name2" }
            // , new { Price = 101, Name = "name3" }
            //  , new { Price = 1000, Name = "name4" },
            //     new { Price = 10000, Name = "name5" }
            //, new { Price = 100000, Name = "name6" }

            //  };
            var list = new List<string>();
            ExcelCreatorEPPlus excelCreator= new ExcelCreatorEPPlus();
            var tree = new ColumnTree
            {
                Roots = new List<ColumnTreeNode>{
                 new ColumnTreeNode{ Title="商品分类", Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="供应商", Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="商品名称"  },
                 new ColumnTreeNode{ Title="品牌", Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="计量单位", Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="规格型号",   },
                 new ColumnTreeNode{ Title="商品类型", Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="含税供货价",   },
                 new ColumnTreeNode{ Title="含税运供货价" },
                 new ColumnTreeNode{ Title="市场参考价"},
                 new ColumnTreeNode{ Title="比价均值"},
                 new ColumnTreeNode{ Title="商品广告"},
                 new ColumnTreeNode{ Title="供货周期"},
                 new ColumnTreeNode{ Title="样品到货周期"},
                 new ColumnTreeNode{ Title="发货地点"},
                 new ColumnTreeNode{ Title="其他平台链接"},
                 new ColumnTreeNode{ Title="商品参数"},
                 new ColumnTreeNode{ Title="功能特点"},
                 new ColumnTreeNode{ Title="品牌介绍"},
                 new ColumnTreeNode{ Title="是否3C商品",Candidates=new List<string>{ "是","否"} },
                 new ColumnTreeNode{ Title="3C证书编号"},
                 new ColumnTreeNode{ Title="3C认证时间"},
                 new ColumnTreeNode{ Title="3C到期时间"},
                }
            };
            var stream = excelCreator.Create(list, tree);
            using (var file = new FileStream("CreateWithCandidates" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }
        }


    }
}