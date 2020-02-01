﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountMergedImageInterfaceWithTargetAttrTest
    {
        [TestMethod]
        public void ExecuteTest()
        {
            var TEST_NAME = "The name";
            var TEST_ACCOUNT = "12345";

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountMergedImageInterfaceWithTargetAttr>())
            {
                var account = new Entities.Account { AccountId = Guid.NewGuid() };
                account.AccountNumber = TEST_ACCOUNT;
                ctx.AddEntity(account);

                var target = new Entities.Account { AccountId = account.AccountId.Value, Name = TEST_NAME };
                ctx.Update(target);

                account = ctx.GetEntityById<Entities.Account>(account.AccountId.Value);

                Assert.AreEqual($"{account.AccountNumber}|{account.Name}", account.Description);
            }
        }

    }
}
