using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.UnitTests.Plugins.Template
{
    [TestClass]
    public class TemplatePluginTest
    {
        [TestMethod]
        public void OnPostCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Template.TemplatePlugin>())
            {
                var template = new Entities.Template { TemplateId = Guid.NewGuid(), Title = "Test Template" };

                var called = false;
                ctx.OnPost = delegate
                {
                    called = true;
                };

                ctx.Create(template);

                Assert.IsTrue(called);
            }
        }
    }
}
