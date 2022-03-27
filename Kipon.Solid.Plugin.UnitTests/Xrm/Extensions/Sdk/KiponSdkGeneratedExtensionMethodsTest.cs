using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Extensions.Sdk
{
    [TestClass]
    public class KiponSdkGeneratedExtensionMethodsTest : BaseTest
    {
        [TestMethod]
        public void ToEarlyBoundEntityTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity(Entities.Account.EntityLogicalName, Guid.NewGuid());
            entity["name"] = "The name of the company";

            var acc = entity.ToEarlyBoundEntity();

            Assert.AreEqual(typeof(Entities.Account), acc.GetType());

            var ra = acc as Kipon.Solid.Plugin.Entities.Account;
            Assert.AreEqual(ra.Name, entity["name"]);
        }

        [TestMethod]
        public void UnknownEntityTypeExceptionTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity("notdefinedentityname", Guid.NewGuid());
            entity["name"] = "The name of the company";

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnknownEntityTypeException>(() => entity.ToEarlyBoundEntity());
        }


        [TestMethod]
        public void PreValueOfTest()
        {
            var ctx = new Kipon.Solid.Plugin.UnitTests.Mocks.PluginExecutionContext
            {
                PreEntityImages = new Microsoft.Xrm.Sdk.EntityImageCollection()
            };

            var account = new Microsoft.Xrm.Sdk.Entity(Kipon.Solid.Plugin.Entities.Account.EntityLogicalName, Guid.NewGuid());
            account["accountcategorycode"] = new Microsoft.Xrm.Sdk.OptionSetValue(1);
            account["name"] = "Kipon ApS";
            var parent = new Microsoft.Xrm.Sdk.EntityReference("account", new Guid());
            account["parentaccountid"] = parent;
            ctx.PreEntityImages.Add("animage", account);

            var enumValue = ctx.PreValueOf<Kipon.Solid.Plugin.Entities.Account.AnEnum?>(nameof(Kipon.Solid.Plugin.Entities.Account.AnEnumValue));
            Assert.AreEqual(Kipon.Solid.Plugin.Entities.Account.AnEnum.Value1, enumValue);
            Assert.AreEqual(account["name"], ctx.PreValueOf<string>(nameof(Kipon.Solid.Plugin.Entities.Account.Name)));

            Assert.AreEqual(parent, ctx.PreValueOf<Microsoft.Xrm.Sdk.EntityReference>(nameof(Kipon.Solid.Plugin.Entities.Account.ParentAccountId)));

            account.Attributes.Remove("accountcategorycode");
            enumValue = ctx.PreValueOf<Kipon.Solid.Plugin.Entities.Account.AnEnum?>(nameof(Kipon.Solid.Plugin.Entities.Account.AnEnumValue));
            Assert.IsNull(enumValue);

            {
                var ex = Assert.ThrowsException<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(() =>
                {
                    ctx.PreValueOf<string>("NoneExistingProperty");
                });

                Assert.AreEqual(string.Format(Kipon.Xrm.Extensions.Sdk.KiponSdkGeneratedExtensionMethods.UNDEFINED_ATTRIBUTE_MESSAGE, "account", "NoneExistingProperty"), ex.Message);
            }

            {
                var ex = Assert.ThrowsException<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(() =>
                {
                    ctx.PreValueOf<string>(nameof(Kipon.Solid.Plugin.Entities.Account.NoDecorationProperty));
                });

                Assert.AreEqual(string.Format(Kipon.Xrm.Extensions.Sdk.KiponSdkGeneratedExtensionMethods.MISSING_DECORATION_ATTRIBUTE_MESSAGE, "account", nameof(Kipon.Solid.Plugin.Entities.Account.NoDecorationProperty)), ex.Message);
            }
        }

        [TestMethod]
        public void PluginExecutionContextParentParmetersTest()
        {
            var types = Kipon.Xrm.Reflection.Types.Instance;
            var title = "The name";
            types.SetAssembly(typeof(Kipon.Solid.Plugin.Actions.IAccountCountContactsRequest).Assembly);

            var accountId = Guid.NewGuid();
            var ctx = new Kipon.Xrm.Fake.Services.PluginExecutionContext(1, 1, "kipon_AccountCountContacts", "account", accountId, false);

            ctx.AddInput(nameof(Kipon.Solid.Plugin.Actions.IAccountCountContactsRequest.Name), title);
            ctx.AddInput("Target", new Microsoft.Xrm.Sdk.EntityReference("account", accountId));
            var child = ctx.Next(1, 1, "Create", "contact", Guid.NewGuid(), false);

            {
                var parentInput = child.ParentInputParameters<Kipon.Solid.Plugin.Actions.IAccountCountContactsRequest>("account", "kipon_AccountCountContacts");

                Assert.AreEqual(accountId, parentInput.Target.Id);
                Assert.AreEqual(title, parentInput.Name);
            }

            {
                var parentInput = child.ParentInputParameters<MyParam>("account", "kipon_AccountCountContacts");

                Assert.AreEqual(accountId, parentInput.Target.Id);
                Assert.AreEqual(title, parentInput.Name);
            }

            {
                var name = child.ParentInputParameter<string>("account", "kipon_AccountCountContacts", "Name");
                Assert.AreEqual(title, name);
            }
        }

        [TestMethod]
        public void IsOnlyPayloadTest()
        {
            var account = new Kipon.Solid.Plugin.Entities.Account
            {
                AccountId = Guid.NewGuid(),
                Name = "Name",
                Address1_City = "CPH",
                ["createdby"] = new Microsoft.Xrm.Sdk.EntityReference(Entities.SystemUser.EntityLogicalName, Guid.NewGuid()),
                ["createdon"] = System.DateTime.Now,
                ["modifiedby"] = new Microsoft.Xrm.Sdk.EntityReference(Entities.SystemUser.EntityLogicalName, Guid.NewGuid()),
                ["modifiedon"] = System.DateTime.Now
            };
            Assert.IsTrue(account.IsOnlyPayload(nameof(account.Name), nameof(account.Address1_City)));
            Assert.IsFalse(account.IsOnlyPayload(nameof(account.Name)));
        }

        [TestMethod]
        public void OptionSetToEnumTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity();
            entity["enumvalue"] = new Microsoft.Xrm.Sdk.OptionSetValue(1);
            Assert.AreEqual(TestEnum.V1, entity.Attributes.ValueOf<TestEnum?>("enumvalue"));

            TestEnum? value = entity.Attributes.ValueOf<TestEnum?>("notinthere");

            Assert.IsNull(value);

        }

        [TestMethod]
        public void OptionSetCollectionToArrayTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity();

            var col = new Microsoft.Xrm.Sdk.OptionSetValueCollection();
            col.Add(new Microsoft.Xrm.Sdk.OptionSetValue(1));
            col.Add(new Microsoft.Xrm.Sdk.OptionSetValue(2));

            entity["enumvaluecol"] = col;

            var enumcol = entity.Attributes.ValueOf<TestEnum[]>("enumvaluecol");

            Assert.AreEqual(TestEnum.V1, enumcol[0]);
            Assert.AreEqual(TestEnum.V2, enumcol[1]);

            TestEnum[] notin = entity.Attributes.ValueOf<TestEnum[]>("notinthere");

            Assert.IsNull(notin);

        }

        public enum TestEnum
        {
            V1 = 1,
            V2 = 2,
            V3 = 3
        }


        public class MyParam
        {
            public string Name { get; set; }
            public Microsoft.Xrm.Sdk.EntityReference Target { get; set; }
        }
    }
}
