using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Extensions.KiponSdk
{
    [TestClass]
    public class KiponSdkExtensionsTest
    {
        [TestMethod]
        public void ConvertBetweenLogicalNameAndObjectTypeCodeTest()
        {
            Assert.AreEqual(Entities.Account.EntityTypeCode, Entities.Account.EntityLogicalName.ToEntityTypeCode());
            Assert.AreEqual(Entities.Account.EntityLogicalName, Entities.Account.EntityTypeCode.ToEntityLogicalName());
        }
    }
}
