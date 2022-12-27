using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.Strings;

namespace Kipon.Xrm.UnitTests.Xrm.Extensions.Strings
{
    [TestClass]
    public class StringMethodsTest: BaseTest
    {
        [TestMethod]
        public void FirstToUpperTest()
        {
            Assert.AreEqual("kjeLD".FirstToUpper(), "Kjeld");
        }
    }
}
