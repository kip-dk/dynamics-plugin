using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.Generic
{
    [TestClass]
    public class ProspectPluginTest
    {
        [TestMethod]
        public void OnPreUpdateAccountTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Generic.ProspectPlugin>())
            {
                var accountid = Guid.NewGuid();
                ctx.AddEntity(new Entities.Account { AccountId = accountid, Name = "Kipon ApS" });

                ctx.OnPre += delegate ()
                {
                    var acc = ctx.GetEntityById<Entities.Account>(accountid);
                    Assert.IsTrue(acc.CreditLimit.Value == -99M);
                };

                ctx.Update(new Entities.Account { AccountId = accountid, CreditLimit = new Microsoft.Xrm.Sdk.Money(123M) });
            }
        }


        [TestMethod]
        public void OnPreUpdateContactTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Generic.ProspectPlugin>())
            {
                var contactid = Guid.NewGuid();
                ctx.AddEntity(new Entities.Contact { ContactId = contactid, FirstName = "Kjeld", LastName = "Poulsen" });

                ctx.OnPre += delegate ()
                {
                    var con = ctx.GetEntityById<Entities.Contact>(contactid);
                    Assert.IsTrue(con.CreditLimit.Value == -97M);
                };

                ctx.Update(new Entities.Contact { ContactId = contactid, CreditLimit = new Microsoft.Xrm.Sdk.Money(123M) });
            }
        }

    }
}
