using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class CommonPropertyCacheTest : BaseTest
    {
        [TestMethod]
        public void ForTypeTest()
        {
            var pCache = Kipon.Xrm.Reflection.CommonProperty.ForType(typeof(Kipon.Online.Plugin.Entities.Account.IAccountNameChanged), typeof(Kipon.Online.Plugin.Entities.Account));
            Assert.AreEqual(1, pCache.Length);
            Assert.AreEqual("name", pCache[0].LogicalName);
            Assert.IsTrue(pCache[0].Required);
        }
    }
}
