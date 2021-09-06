using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Account
{
    [TestClass]
    public class SpecialAccountPluginTest
    {
        [TestMethod]
        public void OnValidateCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.SpecialAccountPlugin>())
            {
                ctx.OnValidation = delegate
                {
                };

                var target = new Entities.Account { AccountId = Guid.NewGuid(), AccountRatingCode = new Microsoft.Xrm.Sdk.OptionSetValue(2) };
                ctx.Create(target);
            }
        }
    }
}
