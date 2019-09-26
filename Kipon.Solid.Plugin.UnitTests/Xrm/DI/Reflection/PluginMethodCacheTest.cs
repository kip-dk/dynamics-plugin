using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.DI.Reflection
{
    [TestClass]
    public class PluginMethodCacheTest
    {
        [TestMethod]
        public void ForTypePostCreateDuckTypeTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(DuckPluginPostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(3, methods.Length);
            Assert.AreEqual(1, methods[0].Sort);
            Assert.AreEqual(1, methods[0].Parameters.Length);

            Assert.AreEqual(10, methods[1].Sort);
            Assert.AreEqual(20, methods[2].Sort);
            Assert.AreEqual(2, methods[2].Parameters.Length);
        }

        [TestMethod]
        public void ForTypePostCreateDecoratedTypeTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(DecoratedPostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);
            Assert.AreEqual(1, methods[1].Parameters.Length);
        }

        [TestMethod]
        public void ForTypePostCreateMixedStyleTypeTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(MixedStylePostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);
            Assert.AreEqual(1, methods[1].Parameters.Length);
        }

        [TestMethod]
        public void ForTypeMultiPurposePluginTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(MultiPurposePlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);

            methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(MultiPurposePlugin), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(3, methods[0].Parameters.Length);

        }

        [TestMethod]
        public void ForTypeFilteredAttributeTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(FilteredAttributePlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.IsFalse(methods[0].FilterAllProperties);
            Assert.AreEqual(1, methods[0].FilteredProperties.Length);
            Assert.AreEqual(2, methods[1].FilteredProperties.Length);
        }


        [TestMethod]
        public void WrongTargetTypeTest()
        {
            // TO DO -- below will throw unavailableimage by design ... test that it works
            //var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(WrongTargetPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
        }

        [TestMethod]
        public void OrganizationServiceResolveTest()
        {
            var methods = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(BothOrgService), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);

            Assert.IsTrue(methods[0].Parameters[1].RequireAdminService);
            Assert.IsFalse(methods[0].Parameters[2].RequireAdminService);
        }

        public class WrongTargetPlugin
        {
            [Sort(10)]
            public void OnPreCreate(Entities.AccountReference account)
            {
            }
        }


        public class BothOrgService
        {
            public void OnPreCreate(
                Entities.IAccountTarget account, 
                [Kipon.Xrm.Attributes.Admin]Microsoft.Xrm.Sdk.IOrganizationService adminOrgService,
                Microsoft.Xrm.Sdk.IOrganizationService orgService)
            {
            }
        }

        public class FilteredAttributePlugin
        {
            [Sort(10)]
            public void OnPreUpdate(Entities.IOpenRevenueChanged ac)
            {
            }

            [Sort(20)]
            public void OnPreUpdate(Entities.IOpenRevenueChanged ac, Entities.IAccountNameChanged nc)
            {
            }

        }

        public class MultiPurposePlugin
        {
            public void OnPreUpdate(Entities.IAccountTarget tg, Entities.IAccountPreimage pre)
            {
            }

            public void OnPostUpdate(Entities.IAccountTarget tg, Entities.IAccountPreimage pre, Entities.IAccountPostimage post)
            {
            }
        }


        public class MixedStylePostCreate
        {
            [Sort(10)]
            [Step(StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create, Entities.Account.EntityLogicalName)]
            public void NameDoesNotConformToAnything(Entities.IAccountTarget account, Entities.IAccountPostimage thepost)
            {
            }

            [Sort(20)]
            public void OnPostCreate(Entities.IAccountPostimage am)
            {
            }

            public void NotAPluginMethod(Entities.IAccountTarget account)
            {
            }
        }

        public class DecoratedPostCreate
        {
            [Sort(10)]
            [Step(StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create, Entities.Account.EntityLogicalName)]
            public void NameDoesNotConformToAnything10(Entities.IAccountTarget account, Entities.IAccountPostimage thepost)
            {
            }

            [Sort(10)]
            [Step(StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create, Entities.Account.EntityLogicalName)]
            public void NameDoesNotConformToAnything20(Entities.IAccountTarget account)
            {
            }

            public void NotAPluginMethod(Entities.IAccountTarget account)
            {
            }
        }

        public class DuckPluginPostCreate
        {
            public void OnPostCreate(Entities.IAccountPostimage am)
            {
            }

            [Sort(10)]
            public void OnPostCreate(Entities.IAccountTarget account)
            {
            }

            [Sort(20)]
            public void OnPostCreate(Entities.IAccountTarget account, Entities.IAccountPostimage pre)
            {
            }

            public void NotAPluginMethod(Entities.IAccountTarget account)
            {
            }
        }
    }
}
