using Kipon.Xrm.Fake.Repository;

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
        public void UniqueKeyQueryTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountCreatePlugin>())
            {
                var id = Guid.NewGuid();
                ctx.AddEntity(new Entities.Account { AccountId = id, Name = "Kurt" });

                var accountQuery = ctx.GetQuery<Entities.Account>();
                var kurt = (from a in accountQuery where a.AccountId == id select a).Single();

                Assert.AreEqual("Kurt", kurt.Name);
            }
        }

        [TestMethod]
        public void TestForeignKeyQueryTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountCreatePlugin>())
            {
                var id = Guid.NewGuid();
                var primaryContactId = new Microsoft.Xrm.Sdk.EntityReference { LogicalName = Entities.Contact.EntityLogicalName, Id = Guid.NewGuid() };
                ctx.AddEntity(new Entities.Account { AccountId = id, Name = "Kurt", PrimaryContactId = primaryContactId });

                var accountQuery = ctx.GetQuery<Entities.Account>();
                var kurt = (from a in accountQuery where a.PrimaryContactId.Id == primaryContactId.Id select a).Single();

                Assert.AreEqual("Kurt", kurt.Name);

                kurt = (from a in accountQuery where a.PrimaryContactId == primaryContactId select a).Single();

                Assert.AreEqual("Kurt", kurt.Name);

            }
        }


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

        [TestMethod]
        public void TwoInnerJoinEntityQueryTest()
        {
            using (var ctx = PluginExecutionFakeContext.ForType<Kipon.Solid.Plugin.Plugins.Account.AccountCreatePlugin>())
            {
                var a1 = new Entities.Account { AccountId = Guid.NewGuid(), Name = "Kurt A/S" };
                var a2 = new Entities.Account { AccountId = Guid.NewGuid(), Name = "Lars ApS" };

                var c1 = new Entities.Contact { ContactId = Guid.NewGuid(), FirstName = "Kurt", LastName = "Jensen", ParentCustomerId = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, a1.AccountId.Value) };
                var c2 = new Entities.Contact { ContactId = Guid.NewGuid(), FirstName = "Kurt", LastName = "Vilhjem", ParentCustomerId = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, a1.AccountId.Value) };
                var c3 = new Entities.Contact { ContactId = Guid.NewGuid(), FirstName = "Lars", LastName = "Madsen", ParentCustomerId = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, a2.AccountId.Value) };

                ctx.AddEntity(a1);
                ctx.AddEntity(a2);

                ctx.AddEntity(c1);
                ctx.AddEntity(c2);
                ctx.AddEntity(c3);

                var accountQuery = ctx.GetQuery<Entities.Account>();
                var contactQuery = ctx.GetQuery<Entities.Contact>();
                var kurt = (from a in accountQuery
                            join c in contactQuery on a.AccountId equals c.ParentCustomerId.Id
                            where a.Name == "Kurt A/S"
                            select new
                            {
                                Name = a.Name,
                                Firstname = c.FirstName,
                                Lastname = c.LastName
                            }).ToArray().OrderBy(r => r.Lastname).ToArray();

                Assert.AreEqual(2, kurt.Length);

                Assert.AreEqual("Kurt A/S", kurt.First().Name);
                Assert.AreEqual("Jensen", kurt.First().Lastname);
                Assert.AreEqual("Vilhjem", kurt.Last().Lastname);
            }
        }
    }
}
