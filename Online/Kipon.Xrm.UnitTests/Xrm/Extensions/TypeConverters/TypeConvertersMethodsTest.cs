using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.TypeConverters;
using Microsoft.Xrm.Sdk;
using Kipon.Online.Plugin.Entities;

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


        [TestMethod]
        public void IsSameTest()
        {
            object o1 = null;
            object o2 = null;

            Assert.IsTrue(o1.IsSame(o2));

            this.IsSame(new Money(10.2M), new Money(10.2M));
            this.IsSame(new Money(10.2M), 10.2M);
            this.IsSame(10.2M, new Money(10.2M));

            this.IsSame(new OptionSetValue(2), new OptionSetValue(2));
            this.IsSame(2, new OptionSetValue(2));
            this.IsSame(new OptionSetValue(2), 2);

            this.IsSame(new EntityReference(Account.EntityLogicalName, 1.ToGuid()), new EntityReference(Account.EntityLogicalName, 1.ToGuid()));
            this.IsSame(1.ToGuid(), new EntityReference(Account.EntityLogicalName, 1.ToGuid()));
            this.IsSame(new EntityReference(Account.EntityLogicalName, 1.ToGuid()), 1.ToGuid());
        }

        private void IsSame(object o1, object o2)
        {
            Assert.IsTrue(o1.IsSame(o2));
        }
    }
}
