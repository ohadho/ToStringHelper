using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PrintObj;
using TypeMock.ArrangeActAssert;

namespace Tests
{
    public class FieldsClass
    {
        public int publicField = 1;
        private int privateField = 2;

        public static int publicStatic = 0;
        private static int privateStatic = 1;

        [PrintObjIgnore]
        public int fieldToIgnore = 1;
    }

    public class PropertiesClass
    {
        private int fieldProperty;
        public PropertiesClass()
        {
            PublicAutoProp = 1;
        }


        public int PublicProp
        {
            get { return fieldProperty; }
            set { fieldProperty = value; }
        }

        public int PublicAutoProp { get; set; }
    }


    [TestFixture, Isolated]
    public class PrinterTests
    {
        [Test]
        public void TestPublicField()
        {
            var s = Printer.Print().PrintObj(new FieldsClass());
            Assert.IsTrue(s.Contains("publicField: 1"));
        }

        [Test]
        public void TestPrivateField()
        {
            var s = Printer.Print().PrintObj(new FieldsClass());
            Console.WriteLine(s);
            Assert.IsTrue(s.Contains("privateField: 2"));
        }

        [Test]
        public void TestPublicStaticField()
        {
            var s = Printer.Print().PrintObj(new FieldsClass());
            Console.WriteLine(s);
            Assert.IsFalse(s.Contains("fieldToIgnore: 1"));
        }

        [Test]
        public void TestPrivateStaticField()
        {
            var s = Printer.Print().PrintObj(new FieldsClass());
            Console.WriteLine(s);
            Assert.IsTrue(s.Contains("privateStatic: 1"));
        }

        [Test]
        public void TestIgnoreAttirbute()
        {
            var s = Printer.Print().PrintObj(new FieldsClass());
            Console.WriteLine(s);
            Assert.IsFalse(s.Contains("fieldToIgnore"));
        }

        [Test]
        public void TestPublicAutoProperty()
        {
            var s = Printer.Print().PrintObj(new PropertiesClass());
            Console.WriteLine(s);
            Assert.IsTrue(s.Contains("PublicAutoProp: 1"));
        }
    }
}
