using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Fake.Xrm.Reflection
{
    [TestClass]
    public class TypesTest : BaseTest
    {
        [TestMethod]
        public void SetAssemblyTest()
        {
            var types = Kipon.Fake.Xrm.Reflection.Types.Instance;
            types.SetAssembly(typeof(Kipon.Xrm.BasePlugin).Assembly);

            Assert.AreEqual(types.AdminAttribute, typeof(Kipon.Xrm.Attributes.AdminAttribute));
        }
    }
}
