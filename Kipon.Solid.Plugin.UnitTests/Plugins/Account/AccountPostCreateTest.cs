using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountPostCreateTest
    {
        [TestMethod]
        public void OnPostCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountPostCreate>())
            {
                var account = new Entities.Account { AccountId = Guid.NewGuid(), Name = "Name" };

                ctx.OnPost = delegate
                {
                };

                ctx.Create(account);
            }
        }
    }
}
