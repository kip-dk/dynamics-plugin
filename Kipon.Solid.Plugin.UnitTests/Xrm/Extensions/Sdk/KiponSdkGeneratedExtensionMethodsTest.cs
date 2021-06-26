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
    }
}
