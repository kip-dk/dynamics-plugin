using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.TypeConverters;

namespace Kipon.Xrm.UnitTests.Xrm.Extensions.TypeConverters
{
    [TestClass]
    public class TypeConvertersMethodsTest : BaseTest
    {
        [TestMethod]
        public void ToGuidTest()
        {
            Assert.AreEqual(new Guid("00000001-0000-0000-0000-000000000000"), 1.ToGuid());
            Assert.AreEqual(new Guid("00000001-0002-0003-0004-000000000005"), 1.ToGuid(2,3,4,5));
        }

        [TestMethod]
        public void ToIdTest()
        {
            Assert.AreEqual(new Guid("00000001-0000-0000-0000-000000000015"), 1.ToId(15));
        }

    }
}
