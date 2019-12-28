using Kipon.Xrm.Fake.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Fake.Repository
{
    [TestClass]
    public class PluginExecutionFakeContextTest
    {
        #region create test
        [TestMethod]
        public void CreateTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountCreatePlugin>())
            {
                ctx.AddEntity(new Entities.Account { AccountId = Guid.NewGuid(), Name = "Jens" });

                var target = new Entities.Account
                {
                    Name = "kurt"
                };

                ctx.OnValidationCreate = delegate ()
                {
                    Assert.IsNotNull(target.AccountId);
                    Assert.AreEqual(100M, target.CreditLimit.Value);
                    Assert.AreEqual("Jens", target.Name);
                };

                ctx.Create(target);
            }
        }
        #endregion

        #region preupdate test
        [TestMethod]
        public void PreUpdateTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountPlugin>())
            {
                var pre = new Entities.Account
                {
                    AccountId = Guid.NewGuid(),
                    Name = "Prename"
                };
                ctx.AddEntity(pre);

                var target = new Entities.Account
                {
                    AccountId = pre.AccountId,
                    Name = "the next name"
                };

                ctx.OnPreUpdate = delegate ()
                {
                    var result = ctx.GetEntityById<Entities.Account>(pre.AccountId.Value);
                    Assert.AreEqual("The Next Name", result.Name);
                    Assert.AreEqual(pre.Name, result.Description);
                };

                ctx.Update(target);
            }
        }
        #endregion

        #region getquery test
        [TestMethod]
        public void GetQueryTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountPlugin>())
            {
                var query = ctx.GetQuery<Entities.Contact>();
                Assert.IsNotNull(query);
            }
        }
        #endregion

    }
}
