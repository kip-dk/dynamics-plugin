using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class TargetAsPreValueTest
    {
        [TestMethod]
        public void PostTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.TargetAsPreValue>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid(), Name = Kipon.Online.Plugin.Plugins.Account.TargetAsPreValue.EXPECTED_PRE_VALUE };
                ctx.AddEntity(account);

                ctx.Update(new Kipon.Online.Plugin.Entities.Account { AccountId = account.AccountId.Value, Name = Kipon.Online.Plugin.Plugins.Account.TargetAsPreValue.EXPECTED_NEW_VALUE });
            }
        }
    }
}
