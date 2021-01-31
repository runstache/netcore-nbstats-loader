using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NbaStats.Loader.Helpers;

namespace NbaStats.Loader.Tests.Helpers
{
    [TestFixture]
    public class DataTypeHelperTests
    {

        [Test]
        public void TestConvertInteger()
        {
            int result = DataTypeHelper.ConvertToInteger("25");
            Assert.AreEqual(25, result);
        }

        [Test]
        public void TestConvertIntegerFailure()
        {
            int result = DataTypeHelper.ConvertToInteger("BRAD");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestConvertToDouble()
        {
            double result = DataTypeHelper.ConverToDouble("5.6");
            Assert.AreEqual(5.6, result);
        }

        [Test]
        public void TestConvertToDoubleFailure()
        {
            double result = DataTypeHelper.ConverToDouble("BRAD");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestConvertToLong()
        {
            long result = DataTypeHelper.ConvertToLong("25000000000");
            Assert.AreEqual(25000000000, result);
        }

        [Test]
        public void TestConvertToLongFailure()
        {
            long result = DataTypeHelper.ConvertToLong("BRAD");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestConvertDateTime()
        {
            DateTime expected = new DateTime(2020, 1, 24, 13,0,0);
            DateTime result = DataTypeHelper.ConvertDateTime("2020-01-24T18:00Z");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestConvertDateTimeFailure()
        {
            DateTime result = DataTypeHelper.ConvertDateTime("BRAD");
            Assert.AreEqual(default(DateTime), result);
        }

        [Test]
        public void TestConvertDateTimeInjuryFormat()
        {
            DateTime result = DataTypeHelper.ConvertDateTime("Jan 30");

            Assert.AreEqual(30, result.Day);
            Assert.AreEqual(1, result.Month);
            Assert.AreEqual(DateTime.Now.Year, result.Year);
        }       
    }
}
