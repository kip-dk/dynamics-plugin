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
    }
}
