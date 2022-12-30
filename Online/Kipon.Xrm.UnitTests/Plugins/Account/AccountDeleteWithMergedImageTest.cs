using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public  class AccountDeleteWithMergedImageTest
    {
        [TestMethod]
        public void TestMethod()
        {
            Kipon.Online.Plugin.Plugins.Account.AccountDeleteWithMergedImage.TEST_CONTENT = null;
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountDeleteWithMergedImage>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid(), AccountNumber = "123" ,Name = "Test Account" };
                ctx.AddEntity(account);

                ctx.Delete(account.ToEntityReference());

                Assert.AreEqual(Kipon.Online.Plugin.Plugins.Account.AccountDeleteWithMergedImage.TEST_CONTENT, $"{account.AccountNumber}:{account.Name}");
            }
        }
    }
}
