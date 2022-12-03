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
using static PHLibrary.Reflection.ColumnMapCreator;

namespace PHLibrary.ExcelExport.Tests
{
    [TestClass()]
    public class ExcelCreatorEPPlusTests
    {
        public class TestItem
        {
            [Column("姓名")]
            [PropertyOrder(1)]
            public string Name { get; set; }
            [Column("年龄")]
            [PropertyOrder(1)]
            public int Age { get; set; }
            [ImageColumn]
            [PropertyOrder(1)]
            [Column("图片")]
            public string Picture { get; set; }
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
            var stream = excelCreator.Create(
                list,
                new List<ColumnDefine> {
                     new ColumnDefine("age","年龄" ),
                     new ColumnDefine("name","姓名" ),
                     ColumnDefine.ImageColumn("picture","tupian" )
                     }, "sheet1",
                 null, null, "F3");
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

            [PropertyOrder(1)]
            public string PGuid { get; set; }


            [Column("货号")]
            public string Code { get; set; }

            [Column("单价")]
            [PropertyOrder(1)]
            [CustomAmountFormat]
            public long Price { get; set; }

            [Column("图片")]
            [PropertyOrder(4)]
            [ImageColumn]
            public string Picture { get; set; }
            [Column("颜色")]
            public string Color { get; set; }
            [TwoDimensional(true)]
            [PropertyOrder(3)]
            public string Size { get; set; }
            [TwoDimensionalGuid]
            public Guid SizeGuid { get; set; }
            [TwoDimensional]
            public int Amount { get; set; }

        }
        [TestMethod()]
        public void CreateForImagesAndTwoDimetionalData()
        {

            Guid guidM = Guid.NewGuid();
            Guid guidM2 = Guid.NewGuid();
            Guid guidL = Guid.NewGuid();
            Guid guidXL = Guid.NewGuid();
            Guid guidXXL = Guid.NewGuid();
            Guid guidXXXL = Guid.NewGuid();
            Guid guidL2 = Guid.NewGuid();
            var list = new List<Order1103>() {
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM ,  Amount=1,Price=12 },
                 new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM ,  Amount=1,Price=13 },
                  new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM2 ,  Amount=8,Price=13 },

                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="L",SizeGuid=guidL,  Amount=2 ,Price=12},
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="XL",SizeGuid=guidXL,  Amount=3 ,Price=123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",SizeGuid=guidM,  Amount=4 ,Price=123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="L",SizeGuid=guidL ,  Amount=5 , Price = 123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                     Size="XXL",SizeGuid=guidXXL,  Amount=6 , Price = 123},
                new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                     Size="XXXL",SizeGuid=guidXXXL,  Amount=7   ,Price=12345},
                //new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                //     Size="L",SizeGuid=guidL2,  Amount=7   ,Price=12345},
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list,
                  new ColumnDefineBuilder()
                    .AddGroupColumn("Name", "品名")
                    .AddHiddenColumn("PGuid", "")
                    .AddAmountColumn("Price", "价格")
                    .AddGroupColumn("Color", "颜色",false)
                    .AddTwoDimensionalColumns("Size", "SizeGuid", "Amount")
                    .AddImageColumn("Picture", "图片")
                  .Build(),
                    "sheet1", Sort,
                null, "F3");
            using (var file = new FileStream("CreateForImagesAndTwoDimetional" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }

        [TestMethod()]
        public void CreateForTwoDimensionsWithoutGuid()
        {

            Guid guidM = Guid.NewGuid();
            Guid guidM2 = Guid.NewGuid();
            Guid guidL = Guid.NewGuid();
            Guid guidXL = Guid.NewGuid();
            Guid guidXXL = Guid.NewGuid();
            Guid guidXXXL = Guid.NewGuid();
            Guid guidL2 = Guid.NewGuid();
            var list = new List<Order1103>() {
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",  Amount=1,Price=12 },
                 new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",   Amount=1,Price=13 },
                  new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",   Amount=8,Price=13 },

                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="L",   Amount=2 ,Price=12},
                new Order1103{ Name="春装001", Code="CZ001", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="XL",   Amount=3 ,Price=123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="M",   Amount=4 ,Price=123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                    Size="L",   Amount=5 , Price = 123},
                new Order1103{ Name="春装001", Code="CZ001", Color="蓝色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                     Size="XXL",  Amount=6 , Price = 123},
                new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                     Size="XXXL",   Amount=7   ,Price=12345},
                //new Order1103{ Name="春装002", Code="CZ002", Color="红色", Picture="https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135",
                //     Size="L",SizeGuid=guidL2,  Amount=7   ,Price=12345},
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(list,
                new ColumnDefineBuilder()
                  .AddGroupColumn("Name", "品名")
                  .AddHiddenColumn("PGuid", "")
                  .AddAmountColumn("Price", "价格")
                   .AddGroupColumn("Color", "颜色")
                   .AddTwoDimensionalColumns("Size", "", "Amount")
                  .AddImageColumn("Picture", "图片")
                  .Build()
              ,
                "sheet1", Sort,
                null, "F3");
            using (var file = new FileStream("CreateForImagesAndTwoDimetional" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }

        private IList<TwoDimensionalValue> Sort(IList<TwoDimensionalValue> columns)
        {
            return columns.Distinct().OrderBy(x => x.Id).ToList();

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
            var stream = excelCreator.Create(list,

                  new List<ColumnDefine> {new ColumnDefine("age","年龄"),
                new ColumnDefine( "name", "姓名"),

              },

                "sheet1", null,
                new List<IList<string>> {
                    new List<string>{"客户","张三" }
                    ,new List<string>{ "电话","13300000000"}
                    ,new List<string>{ "地址","地址111111111" }
                    ,new List<string>{ "总金额","1230"}
                    ,new List<string>{ "备注","已付款"}
                    }

                , "F2");
            using (var file = new FileStream("CreateForImages" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }

        [TestMethod()]
        public void CreateMultiSheets()
        {

            var list = new List<TestItem>() {
                new TestItem{ Age=1, Name="name",Picture=
                "https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135"  }
                };
            var list2 = new List<TestItem>() {
                new TestItem{ Age=2, Name="name2",Picture=
                "https://www.kunming.cn/news/upload/resources/image/2019/12/24/280186.jpg?1577145717135"  }
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(

                    new ExcelExport.SheetData<TestItem, TestItem>
                    {
                        Data1 = list
                        ,
                        PropertiesToDisplay1 = new List<ColumnDefine> { new ColumnDefine("age"), ColumnDefine.ImageColumn("picture") }
                        ,
                        SheetName1 = "表格1"
                        ,
                        Data2 = list2
                        ,
                        PropertiesToDisplay2 = new List<ColumnDefine> { new ColumnDefine("age"), new ColumnDefine("name"), ColumnDefine.ImageColumn("picture") }
                        ,
                        SheetName2 = "表格2"
                    }
                     , null, "F1"

                );
            using (var file = new FileStream("CreateForImages" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
        public class TestWithDatetime
        {
            [PropertyOrder(1)]
            [DateFormat("hh:mm:ss")]
            public DateTime Begin { get; set; }
            [PropertyOrder(2)]
            [DateFormat("yyyyMMdd hh-mm-ss")]
            public DateTime? End { get; set; }
        }
        [TestMethod()]
        public void CreateForDatetime()
        {

            var list = new List<TestWithDatetime>() {
                new TestWithDatetime{ Begin=DateTime.Now},
                new TestWithDatetime{ Begin=DateTime.Now,End=DateTime.Now.AddSeconds(14)}
                };
            ExcelCreatorEPPlus excelCreator
               = new ExcelCreatorEPPlus();
            var stream = excelCreator.Create(
                list
                , new List<ColumnDefine> { new ColumnDefine("begin"), new ColumnDefine("end") }
                , null
                , null

                , null
                , "F2");
            using (var file = new FileStream("CreateForDatetime" + Guid.NewGuid() + ".xlsx", FileMode.Create, FileAccess.Write))
            {
                // stream.Seek(0, SeekOrigin.Begin);
                CopyStream(stream, file);

            }

        }
    }
}