using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.NugetBasedTest.Plugins.Template
{
    [TestClass]
    public class TemplatePluginTest
    {
        [TestMethod]
        public void OnPostCreateTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Template.TemplatePlugin>())
            {
                var template = new Kipon.Solid.Plugin.Entities.Template { TemplateId = Guid.NewGuid(), Title = "Test Template" };

                ctx.OnPost = delegate
                {
                };

                ctx.Create(template);
            }
        }
    }
}
