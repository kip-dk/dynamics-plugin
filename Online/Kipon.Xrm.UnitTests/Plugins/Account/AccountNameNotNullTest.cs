using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountNameNotNullTest
    {
        [TestMethod]
        public void NotNullTest()
        {
            Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.PrePareTest();

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull>())
            {
                Assert.AreEqual(Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);

                ctx.Create(new Kipon.Online.Plugin.Entities.Account { Name = "Kurt" });
                Assert.AreEqual(Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST_AFTER);
            }
        }

        [TestMethod]
        public void IsNullTest()
        {
            Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.PrePareTest();

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull>())
            {
                Assert.AreEqual(Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);

                ctx.Create(new Kipon.Online.Plugin.Entities.Account { Description = "Kurt" });
                Assert.AreEqual(Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST, Kipon.Online.Plugin.Plugins.Account.AccountNameNotNull.TEST_BEFORE);
            }
        }

    }
}
