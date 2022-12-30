using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class SpecialAccountPluginTest
    {
        [TestMethod]
        public void OnValidateCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.SpecialAccountPlugin>())
            {
                ctx.OnValidation = delegate
                {
                };

                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid(), AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(2) };
                ctx.Create(target);
            }
        }

        [TestMethod]
        public void TargetAttributesTest()
        {
            /*
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.UseTargetAttributesPlugin>())
            {
                var target = new Entities.Account { AccountId = Guid.NewGuid(), Name = "A name" };
                ctx.OnPre = delegate
                {
                    Assert.AreEqual("FALSE", target.Description);
                };

                ctx.Create(target);
            }*/

            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.UseTargetAttributesPlugin>())
            {
                var id = Guid.NewGuid();
                var pre = new Kipon.Online.Plugin.Entities.Account { AccountId = id, Name = "A name", Telephone1 = "11111111" };

                ctx.AddEntity(pre);

                var target = new Kipon.Online.Plugin.Entities.Account { AccountId = id, Telephone1 = "22222222" };

                ctx.OnPre = delegate
                {
                    Assert.AreEqual("TRUE 11111111", target.Description);
                };

                ctx.Update(target);
            }
        }
    }
}
