using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountNameNotNullTest
    {
        [TestMethod]
        public void NotNullTest()
        {
            Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.PrePareTest();

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull>())
            {
                Assert.AreEqual(Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);

                ctx.Create(new Entities.Account { Name = "Kurt" });
                Assert.AreEqual(Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST_AFTER);
            }
        }

        [TestMethod]
        public void IsNullTest()
        {
            Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.PrePareTest();

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull>())
            {
                Assert.AreEqual(Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);

                ctx.Create(new Entities.Account { Description = "Kurt" });
                Assert.AreEqual(Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Solid.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);
            }
        }

    }
}
