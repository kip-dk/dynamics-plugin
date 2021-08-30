using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.kipon_datepoc
{
    [TestClass]
    public class PostUpdateTest
    {
        [TestMethod]
        public void ExecuteOnPost()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.kipon_datepoc.kipon_datepocPlugin>())
            {
                var pocDate = new Entities.kipon_datepoc { kipon_datepocId = Guid.NewGuid(), kipon_name = "Kjeld I. Poulsen" };

                ctx.AddEntity(pocDate);

                ctx.OnPost = delegate
                {
                };

                var target = new Entities.kipon_datepoc { kipon_datepocId = pocDate.kipon_datepocId.Value, kipon_dateonly = new DateTime(1964,1,20) };
                ctx.Update(target);
            }
        }

    }
}
