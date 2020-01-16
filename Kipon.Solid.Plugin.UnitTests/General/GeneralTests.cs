using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.General
{
    [TestClass]
    public class GeneralTests : BaseTest
    {
        [TestMethod]
        public void NullStringEqualNullString()
        {
            string x = null;
            string y = null;

            Assert.IsTrue(x == y);
        }

        [TestMethod]
        public void DecimalNullableCastTest()
        {
            object x = null;
            x = 123M;

            var asNullableDecimal = x as decimal?;
            Assert.AreEqual(123M, asNullableDecimal.Value);
        }

        [TestMethod]
        public void DateCompareTest()
        {
            var first = System.DateTime.Today;
            var second = System.DateTime.Today.AddMonths(1);
            var result = Math.Round(((decimal)second.Subtract(first).Days) / (365.25M / 12M), 0);

            Assert.AreEqual(1M, result);

            int v1 = -2;
            var v2 = v1 + 1;
            Assert.AreEqual(-1, v2);
        }
    }
}
