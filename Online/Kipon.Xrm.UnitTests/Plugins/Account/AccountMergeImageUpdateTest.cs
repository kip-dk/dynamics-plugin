﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountMergeImageUpdateTest
    {
        [TestMethod]
        public void ExecuteTest()
        {
            var TEST_VALUE = Kipon.Online.Plugin.Plugins.Account.AccountMergeImageUpdate.TEST_VALUE;

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccountMergeImageUpdate>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid() };
                account.AccountNumber = TEST_VALUE;
                ctx.AddEntity(account);

                ctx.OnPre = delegate
                {
                    var acc = ctx.GetEntityById<Kipon.Online.Plugin.Entities.Account>(account.AccountId.Value);
                    Assert.AreEqual($"Assigned: {TEST_VALUE}", acc.Description);
                    Assert.AreEqual($"Assigned: {TEST_VALUE}", acc.AccountNumber);
                };

                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = account.AccountId.Value, Name = TEST_VALUE };
                ctx.Update(target);
            }

        }
    }
}
