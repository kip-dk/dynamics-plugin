using Kipon.Fake.Xrm.Extensions.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.kipon_datepoc
{
    [TestClass]
    public class kipon_multitestPluginTest
    {
        [TestMethod]
        public void OnPreTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.kipon_multitest.kipon_multitestPlugin>())
            {
                ctx.AddEntity(new Entities.kipon_number { kipon_numberId = 1.ToGuid(), kipon_latest = 22 });
                ctx.AddEntity(new Entities.kipon_number { kipon_numberId = 1.ToGuid(2024), kipon_latest = 70 });

                ctx.AddEntity(new Entities.kipon_multitest { kipon_multitestId = 1.ToGuid(), kipon_name = "test" });

                var target = new Entities.kipon_multitest { kipon_multitestId = 1.ToGuid(), kipon_calculate = true };

                ctx.OnPre += delegate
                {
                    target.kipon_number = 22;
                };

                ctx.Update(target);
            }
        }

    }
}
