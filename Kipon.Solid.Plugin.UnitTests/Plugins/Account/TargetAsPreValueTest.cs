using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class TargetAsPreValueTest
    {
        [TestMethod]
        public void PostTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.TargetAsPreValue>())
            {
                var account = new Entities.Account { AccountId = Guid.NewGuid(), Name = Kipon.Solid.Plugin.Plugins.Account.TargetAsPreValue.EXPECTED_PRE_VALUE };
                ctx.AddEntity(account);

                ctx.Update(new Entities.Account { AccountId = account.AccountId.Value, Name = Kipon.Solid.Plugin.Plugins.Account.TargetAsPreValue.EXPECTED_NEW_VALUE });
            }

        }
    }
}
