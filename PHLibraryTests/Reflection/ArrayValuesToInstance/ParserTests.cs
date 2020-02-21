using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Reflection.ArrayValuesToInstance;
using PHLibrary.Reflection.ArrayValuesToInstance.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Reflection.ArrayValuesToInstance.Tests
{
    [TestClass()]
    public class ArrayValuesToInstanceTests
    {
        [TestMethod()]
        public void ParserOneItemTest()
        {
            string[] values = new string[] { "Lincon", "1988-10-11", "21", "13.2" };

            var convertor = new ArrayValuesToInstance.Parser<Person, string>(
                new PropertyAttributeDeterminer<Person>(values)
                );;
            Person p = convertor.Parse(values);
            Assert.AreEqual("Lincon", p.Name);
            Assert.AreEqual(21, p.Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), p.Birthday);
        }
        [TestMethod()]
        public void ParserWithArrayIndexAttributeOneItemTest()
        {
            var values = new string[,] { { "Name","Birthday","Age","Weight"}, { "Lincon", "1988-10-11", "21", "13.2" }, { "Lincon2", "1988-10-11", "21", "13.2" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate, string>
                (new FirstArrayDeterminer<ClassMate>(new string[]{   "Name", "Birthday", "Age", "Weight"   }));
            var classmates = convertor.ParseList(values);
            Assert.AreEqual(2, classmates.Count);
            Assert.AreEqual("Lincon", classmates[0].Name);
            Assert.AreEqual(21, classmates[0].Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), classmates[0].Birthday);
            Assert.AreEqual("Lincon2", classmates[1].Name);
        }
        [TestMethod()]
        public void FirstArrayPropertyOrderNotMatchPropertyOrder()
        {
            var values = new string[,] { { "Age", "Name", "Birthday", "Weight" }, { "21", "Lincon", "1988-10-11",  "13.2" }, { "22", "Lincon2", "1988-10-12",  "133" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate, string>
                (new FirstArrayDeterminer<ClassMate>(new string[] { "Age", "Name", "Birthday", "Weight" }));
            var classmates = convertor.ParseList(values);
            Assert.AreEqual(2, classmates.Count);
            Assert.AreEqual("Lincon", classmates[0].Name);
            Assert.AreEqual(21, classmates[0].Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), classmates[0].Birthday);
            Assert.AreEqual("Lincon2", classmates[1].Name);
        }
        [TestMethod()]
        public void ParserManyItemTest()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21", "13.2" }, { "Lincon2", "1988-10-11", "21", "13.2" } };
            var convertor = new ArrayValuesToInstance.Parser<Person, string>(new PropertyAttributeDeterminer<Person>(new string[]{   "Lincon", "1988-10-11", "21", "13.2"  }));
            var persons = convertor.ParseList(values);
            Assert.AreEqual(2, persons.Count);
            Assert.AreEqual("Lincon", persons[0].Name);
            Assert.AreEqual(21, persons[0].Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), persons[0].Birthday);
            Assert.AreEqual("Lincon2", persons[1].Name);
        }
        [TestMethod()]
        public void ParserOneHasNoOrderProperty()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21", "13.2" }, { "Lincon2", "1988-10-11", "21", "13.2" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMateNoProperty, string>(
                new PropertyAttributeDeterminer<ClassMateNoProperty>(new string[] { "Lincon", "1988-10-11", "21", "13.2" }));
            var mater2 = convertor.ParseList(values);
            Assert.AreEqual(2, mater2.Count);
            Assert.AreEqual("Lincon", mater2[0].Name);
            Assert.AreEqual(21, mater2[0].Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), mater2[0].Birthday);
            Assert.AreEqual("Lincon2", mater2[1].Name);
        }
        
        [TestMethod()]
        [ExpectedException(typeof(ValuesCountNotMatch))]
        public void PropertiesCountNotMatchValues()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21"  }, { "Lincon2", "1988-10-11", "21"   } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMateNoProperty, string>
                (new PropertyAttributeDeterminer<ClassMateNoProperty>(new string[] { "Lincon", "1988-10-11", "21" }));
          
            
        }
        [TestMethod()]
        [ExpectedException(typeof(OrderOutOfValuesRange))]
        public void OrderOutOfRange()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21" }, { "Lincon2", "1988-10-11", "21" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate4, string>( new PropertyAttributeDeterminer<ClassMate4>(
                new string []{ "Lincon", "1988-10-11", "21" }));
             convertor.ParseList(values);



        }
        [TestMethod()]
        [ExpectedException(typeof(ConvertFailure<string>))]
        public void ConvertFailure()
        {
            var values = new string[,] { { "Lincon", "23a","1988-10-11", "21" }, { "Lincon2", "23", "1988-10-11", "21" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate2, string>(new PropertyAttributeDeterminer<ClassMate2>(
                new string[] { "Lincon", "231", "1988-10-11", "21" }));
            convertor.ParseList(values);

        }
        [TestMethod]
        public void Realdata1Test()
        {
            var values =
                Newtonsoft.Json.JsonConvert.DeserializeObject<string[,]>(
                System.IO.File.ReadAllText(
                    Environment.CurrentDirectory
                    + @"\Reflection\ArrayValuesToInstance\Files\ApiResult1.txt")
                );
            
            var convertor = new ArrayValuesToInstance.Parser<IndexGSMLTE_PS_GEN, string>(new FirstArrayDeterminer<IndexGSMLTE_PS_GEN>(
                new string[] { "TERMINAL_CODE", "UL_2G_TCP_PACKET", "CELL_ID", "TCP_UL_DATE", "DL_2G_TCP_PACKET", "TCP_DL_DATE", "FAILURE_CAUSE", "UL_2G_TCP_RE_PACKETS", "TCPDL_DATA", "TCP_DELAY", "SAEGW", "UL_2G_TCP_DISORDER_PACKETS", "TCP_CONNECT_REQ", "DL_2G_TCP_DISORDER_PACKETS", "TCP_CONNECT_SUC", "DL_2G_TCP_RE_PACKETS", "APN" }));
           var result= convertor.ParseList(values);
            Console.WriteLine(result.Count);

        }
    }
    public class IndexGSMLTE_PS_GEN
    {

        public string TERMINAL_CODE { get; set; }
        public string CELL_ID { get; set; }
        public string APN { get; set; }
        public string SAEGW { get; set; }
        public string FAILURE_CAUSE { get; set; }
        public string UL_2G_TCP_PACKET { get; set; }
        public string DL_2G_TCP_PACKET { get; set; }
        public string UL_2G_TCP_DISORDER_PACKETS { get; set; }
        public string DL_2G_TCP_DISORDER_PACKETS { get; set; }
        public string UL_2G_TCP_RE_PACKETS { get; set; }
        public string DL_2G_TCP_RE_PACKETS { get; set; }
        public string TCPDL_DATA { get; set; }
        public string TCP_DELAY { get; set; }
        public int TCP_CONNECT_REQ { get; set; }
        public int TCP_CONNECT_SUC { get; set; }
        public string TCP_UL_DATE { get; set; }
        public string TCP_DL_DATE { get; set; }

    }
    public class UserActivity
    {
        public string IMSI { get; set; }
        public string IMEI { get; set; }
        public string MSISDN { get; set; }
        public string CELL_ID { get; set; }

        public string INDUSTRY_TYPE { get; set; }
        public string APN_NAME { get; set; }
        public string APN { get; set; }
        public string CellIp { get; set; }
        public string starttime { get; set; }
        public string DeviceName { get; set; }
        public string ServerIp { get; set; }
    }
    public class Person
    {
        public Person() { }
        [PropertyOrder(0)]
        public string Name { get; set; }
        [PropertyOrder(2)]
        public int Age { get; set; }
        [PropertyOrder(1)]
        public DateTime Birthday { get; set; }
        [PropertyOrder(3)]
        public decimal Weight { get; set; }

    }
    
    public class ClassMate
    {
      
        [PropertyOrder(0)]
        public string Name { get; set; }
        [PropertyOrder(2)]
        public int Age { get; set; }
        [PropertyOrder(1)]
        public DateTime Birthday { get; set; }
        [PropertyOrder(3)]
        public decimal Weight { get; set; }

    }
    public class ClassMate2
    {

        
        public string Name { get; set; }
      
        public int Age { get; set; }
       
        public DateTime Birthday { get; set; }
        
        public decimal Weight { get; set; }

    }
    public class ClassMateNoProperty
    {

        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public decimal Weight { get; set; }

    }
    public class ClassMate3
    {

        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }



        public decimal Weight { get; set; }

    }
    public class ClassMate4 { 

        [PropertyOrder(4)]
        public string Name { get; set; }
        [PropertyOrder(1)]
        public DateTime Birthday { get; set; }
        [PropertyOrder(3)]
        public int Age { get; set; }

 

    }
}