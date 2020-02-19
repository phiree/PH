using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHLibrary.Reflection.ArrayValuesToInstance;
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
            var convertor = new ArrayValuesToInstance.Parser<Person, string>();
            Person p = convertor.Parse(values);
            Assert.AreEqual("Lincon", p.Name);
            Assert.AreEqual(21, p.Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), p.Birthday);
        }
        [TestMethod()]
        public void ParserWithArrayIndexAttributeOneItemTest()
        {
            var values = new string[,] { { "Name","Birthday","Age","Weight"}, { "Lincon", "1988-10-11", "21", "13.2" }, { "Lincon2", "1988-10-11", "21", "13.2" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate, string>();
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
            var convertor = new ArrayValuesToInstance.Parser<Person, string>();
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
            var convertor = new ArrayValuesToInstance.Parser<ClassMateNoProperty, string>();
            var mater2 = convertor.ParseList(values);
            Assert.AreEqual(2, mater2.Count);
            Assert.AreEqual("Lincon", mater2[0].Name);
            Assert.AreEqual(21, mater2[0].Age);
            Assert.AreEqual(new DateTime(1988, 10, 11), mater2[0].Birthday);
            Assert.AreEqual("Lincon2", mater2[1].Name);
        }
       
        [TestMethod()]
        public void PropertiesCountNotMatchValues()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21"  }, { "Lincon2", "1988-10-11", "21"   } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMateNoProperty, string>();
            Assert.ThrowsException<Exceptions.ValuesCountNotMatch>(()=>convertor.ParseList(values));
            
        }
        [TestMethod()]
        public void OrderOutOfRange()
        {
            var values = new string[,] { { "Lincon", "1988-10-11", "21" }, { "Lincon2", "1988-10-11", "21" } };
            var convertor = new ArrayValuesToInstance.Parser<ClassMate4, string>();
            Assert.ThrowsException<Exceptions.OrderOutOfValuesRange>(() => convertor.ParseList(values)
            ,"whatis ");
            

        }
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
    [DataStartArrayIndex(1)]
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
    [DataStartArrayIndex(0)]
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
        public DateTime Birthday { get; set; }
        public int Age { get; set; }

 

    }
}