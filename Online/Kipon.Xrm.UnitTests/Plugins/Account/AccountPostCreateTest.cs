using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountPostCreateTest
    {
        [TestMethod]
        public void OnPostCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountPostCreate>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid(), Name = "Name" };

                ctx.OnPost = delegate
                {
                };

                ctx.Create(account);
            }
        }
    }

}
