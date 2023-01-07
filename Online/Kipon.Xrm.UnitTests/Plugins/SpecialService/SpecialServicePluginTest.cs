using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.UnitTests.Plugins.SpecialService
{
    [TestClass]
    public class SpecialServicePluginTest
    {
        [TestMethod]
        public void PluginTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.SpecialService.SpecialServicePlugin>())
            {
                var multi = new Entities.kipon_multitest { kipon_multitestId = Guid.NewGuid() };

                ctx.OnPre = delegate
                {
                    Assert.AreEqual(multi.kipon_Name, typeof(Kipon.Online.Plugin.Entities.AdminCrmUnitOfWork).FullName);
                };

                ctx.Create(multi);
            }
        }
    }
}
