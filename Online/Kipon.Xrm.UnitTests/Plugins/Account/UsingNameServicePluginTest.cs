using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Plugins.Account
{
    [TestClass]
    public class UsingNameServicePluginTest : BaseTest
    {
        [TestMethod]
        public void PluginTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<Kipon.Online.Plugin.Plugins.Account.UsingNameServicePlugin>())
            {
                var contact = new Kipon.Online.Plugin.Entities.Contact
                {
                    ContactId = Guid.NewGuid(),
                    FirstName = "Kjeld",
                    LastName = "Poulsen",
                    ["fullname"] = "Kjeld Poulsen"
                };
                ctx.AddEntity(contact);

                var account = new Kipon.Online.Plugin.Entities.Account
                {
                    AccountId = Guid.NewGuid(),
                    AccountNumber = "123",
                    Name = "Test Account",
                    PrimaryContactId = new Microsoft.Xrm.Sdk.EntityReference(contact.LogicalName, contact.ContactId.Value)
                };

                var called = false;
                ctx.OnPre = delegate
                {
                    Assert.AreEqual(account.Description, contact.FullName);
                    called = true;
                };

                ctx.Create(account);
                Assert.IsTrue(called);
            }

        }
    }
}
