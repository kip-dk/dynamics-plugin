using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountWithConfigPluginTest
    {

        [TestMethod]
        public void ExecuteTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountWithConfigPlugin>(
                Kipon.Online.Plugin.Plugins.Account.AccountWithConfigPlugin.EXPECT_UNSECURE,
                Kipon.Online.Plugin.Plugins.Account.AccountWithConfigPlugin.EXPECT_SECURE))
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid() };
                account.Name = "A test value";


                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = account.AccountId.Value, AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(2) };
                ctx.Create(target);
            }
        }

    }
}
