using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Extensions.Sdk
{
    [TestClass]
    public class KiponSdkGeneratedExtensionMethodsTest
    {
        [TestMethod]
        public void ToEarlyBoundEntityTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity(Entities.Account.EntityLogicalName, Guid.NewGuid());
            entity["name"] = "The name of the company";

            var acc = entity.ToEarlyBoundEntity();

            Assert.AreEqual(typeof(Entities.Account), acc.GetType());

            var ra = acc as Kipon.Solid.Plugin.Entities.Account;
            Assert.AreEqual(ra.Name, entity["name"]);
        }

        [TestMethod]
        public void UnknownEntityTypeExceptionTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity("notdefinedentityname", Guid.NewGuid());
            entity["name"] = "The name of the company";

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnknownEntityTypeException>(() => entity.ToEarlyBoundEntity());
        }
    }
}
