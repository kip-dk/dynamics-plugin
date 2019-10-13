extern alias kiponfake;

using kiponfake::Kipon.Xrm.Fake.Repository;
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
    }
}
