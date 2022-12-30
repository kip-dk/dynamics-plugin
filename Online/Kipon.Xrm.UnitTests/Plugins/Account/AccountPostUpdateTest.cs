using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class AccountPostUpdateTest
    {
        [TestMethod]
        public void ExecuteHitTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccoutPostUpdate>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid() };
                account.AccountNumber = Kipon.Online.Plugin.Plugins.Account.AccoutPostUpdate.TEST_POST_UPDATE_ACCOUNTNUMBER;
                account.Name = "A test value";
                account.AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(1);
                ctx.AddEntity(account);

                Kipon.Online.Plugin.Service.AccountService.POST_MERGE_TEST = null;
                string expect = Kipon.Online.Plugin.Service.AccountService.PostMergedTest(account.Name, new Microsoft.Xrm.Sdk.OptionSetValue(2));

                ctx.OnPost = delegate
                {
                    Assert.AreEqual(Kipon.Online.Plugin.Service.AccountService.POST_MERGE_TEST, expect);
                };


                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = account.AccountId.Value, AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(2) };
                ctx.Update(target);
            }
        }

        [TestMethod]
        public void ExecuteDoNotHitTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.AccoutPostUpdate>())
            {
                var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid() };
                account.AccountNumber = Kipon.Online.Plugin.Plugins.Account.AccoutPostUpdate.TEST_POST_UPDATE_ACCOUNTNUMBER;
                account.Name = "A test value";
                account.AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(1);
                ctx.AddEntity(account);

                Kipon.Online.Plugin.Service.AccountService.POST_MERGE_TEST = "DO_NOT_HIT";
                string expect = "DO_NOT_HIT";

                ctx.OnPost = delegate
                {
                    Assert.AreEqual(Kipon.Online.Plugin.Service.AccountService.POST_MERGE_TEST, expect);
                };


                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = account.AccountId.Value, Address1_City = "København" };
                ctx.Update(target);
            }
        }


    }
}
