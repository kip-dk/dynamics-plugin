using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountWithConfigPluginTest
    {

        [TestMethod]
        public void ExecuteTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountWithConfigPlugin>(
                Kipon.Solid.Plugin.Plugins.Account.AccountWithConfigPlugin.EXPECT_UNSECURE,
                Kipon.Solid.Plugin.Plugins.Account.AccountWithConfigPlugin.EXPECT_SECURE))
            {
                var account = new Entities.Account { AccountId = Guid.NewGuid() };
                account.Name = "A test value";


                var target = new Entities.Account { AccountId = account.AccountId.Value, AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(2) };
                ctx.Create(target);
            }
        }

    }
}
