using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.General
{
    [TestClass]
    public class GeneralTests
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
    }
}
