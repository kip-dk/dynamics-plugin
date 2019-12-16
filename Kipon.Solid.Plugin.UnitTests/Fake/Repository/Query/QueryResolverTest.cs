extern alias kiponfake;
using kiponfake::Kipon.Xrm.Fake.Repository;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Fake.Repository.Query
{
    [TestClass]
    public class QueryResolverTest
    {
        [TestMethod]
        public void SingleEntityQueryTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountCreatePlugin>())
            {
                ctx.AddEntity(new Entities.Account { AccountId = Guid.NewGuid(), Name = "Kurt" });
                ctx.AddEntity(new Entities.Account { AccountId = Guid.NewGuid(), Name = "Lars" });

                var accountQuery = ctx.GetQuery<Entities.Account>();
                var kurt = (from a in accountQuery where a.Name == "Kurt" select a).Single();

                Assert.AreEqual("Kurt", kurt.Name);
            }
        }
    }
}
