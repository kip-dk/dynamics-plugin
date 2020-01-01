using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class PluginMethodCacheTest
    {
        private Kipon.Xrm.Reflection.PluginMethod.Cache pluginMethodcache = new Kipon.Xrm.Reflection.PluginMethod.Cache(typeof(Kipon.Solid.Plugin.Entities.Account).Assembly);

        #region shared entity interface target injection
        [TestMethod]
        public void SharedEntityTargetInterfaceTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Generic.ProspectPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(1, methods[0].Parameters.Length);
            Assert.AreEqual(typeof(Entities.Account), methods[0].Parameters[0].ToType);

            methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Generic.ProspectPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Contact.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(1, methods[0].Parameters.Length);
            Assert.AreEqual(typeof(Entities.Contact), methods[0].Parameters[0].ToType);
        }
        #endregion

        [TestMethod]
        public void ForTypePostCreateDuckTypeTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(DuckPluginPostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
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
            var methods = pluginMethodcache.ForPlugin(typeof(DecoratedPostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);
            Assert.AreEqual(1, methods[1].Parameters.Length);
        }

        [TestMethod]
        public void ForTypePostCreateMixedStyleTypeTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(MixedStylePostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);
            Assert.AreEqual(1, methods[1].Parameters.Length);
        }

        [TestMethod]
        public void ForTypeMultiPurposePluginTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(MultiPurposePlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(2, methods[0].Parameters.Length);

            methods = pluginMethodcache.ForPlugin(typeof(MultiPurposePlugin), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.AreEqual(3, methods[0].Parameters.Length);

        }

        [TestMethod]
        public void ForTypeFilteredAttributeTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(FilteredAttributePlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(2, methods.Length);
            Assert.IsFalse(methods[0].FilterAllProperties);
            Assert.AreEqual(1, methods[0].FilteredProperties.Length);
            Assert.AreEqual(2, methods[1].FilteredProperties.Length);
        }


        #region wrong target
        [TestMethod]
        public void WrongTargetTypeTest()
        {
            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnavailableImageException>(() =>
            {
                var methods = pluginMethodcache.ForPlugin(typeof(WrongTargetPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            });
        }

        public class WrongTargetPlugin
        {
            [Sort(10)]
            public void OnPreCreate(Entities.AccountReference account)
            {
            }
        }
        #endregion


        #region no postimage test
        [TestMethod]
        public void NoPostImageTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Account.AccountPostCreate), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
            Assert.IsFalse(methods[0].HasPostimage());
        }
        #endregion

        #region for RetreiveMultiPostAsync
        [TestMethod]
        public void RetreiveMultiPostAsyncTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Account.AccountRetrieveMultiplePostAsync), (int)StepAttribute.StageEnum.Post, StepAttribute.MessageEnum.RetrieveMultiple.ToString(), Entities.Account.EntityLogicalName, true);
            Assert.AreEqual(1, methods.Length);
        }
        #endregion

        [TestMethod]
        public void OrganizationServiceResolveTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(BothOrgService), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Create.ToString(), Entities.Account.EntityLogicalName, false);

            Assert.IsTrue(methods[0].Parameters[1].RequireAdminService);
            Assert.IsFalse(methods[0].Parameters[2].RequireAdminService);
        }

        #region target relevant test
        [TestMethod]
        public void IsTargetRelevantTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(RelevantAttributePLugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);

            var target = new Entities.Account { AccountId = Guid.NewGuid(), CreditLimit = new Microsoft.Xrm.Sdk.Money(10M) };

            Assert.IsFalse(methods[0].IsRelevant(target));
            Assert.IsTrue(methods[1].IsRelevant(target));
            Assert.IsTrue(methods[2].IsRelevant(target));

            target = new Entities.Account { AccountId = Guid.NewGuid(), Name = null };

            Assert.IsTrue(methods[0].IsRelevant(target));
            Assert.IsFalse(methods[1].IsRelevant(target));
            Assert.IsTrue(methods[2].IsRelevant(target));
        }

        [TestMethod]
        public void UpdatePreMixedPropertiesTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Account.AccountPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Update.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(3, methods.Length);
            Assert.AreEqual(3, methods[0].Parameters.Length);
            Assert.AreEqual(typeof(Entities.Account), methods[0].Parameters[0].ToType);
            Assert.AreEqual(typeof(Entities.Account), methods[0].Parameters[1].ToType);
            Assert.AreEqual(typeof(Service.AccountService), methods[0].Parameters[2].ToType);

            Assert.AreEqual(1, methods[2].FilteredProperties.Length);
        }


        [TestMethod]
        public void DeleteAccountRefTest()
        {
            var methods = pluginMethodcache.ForPlugin(typeof(Kipon.Solid.Plugin.Plugins.Account.AccountPlugin), (int)StepAttribute.StageEnum.Pre, StepAttribute.MessageEnum.Delete.ToString(), Entities.Account.EntityLogicalName, false);
            Assert.AreEqual(1, methods.Length);
        }

        public class RelevantAttributePLugin
        {
            [Sort(10)]
            public void OnPreUpdate(Entities.Account.IAccountNameChanged nameChanged)
            {
            }

            [Sort(20)]
            public void OnPreUpdate(Entities.Account.ICreditLimitChanged creditLimitChanged)
            {
            }

            [Sort(30)]
            public void OnPreUpdate(Entities.Account.ICreditLimitChanged cl, Entities.Account.IAccountNameChanged nc)
            {
            }
        }
        #endregion


        #region preimageproperty test
        [TestMethod]
        public void PreimagePropertyTest()
        {
            var methods = pluginMethodcache.ForPlugin(
                typeof(PreimagePropertyPlugin), 
                (int)StepAttribute.StageEnum.Pre, 
                StepAttribute.MessageEnum.Update.ToString(), 
                Entities.Account.EntityLogicalName, 
                false);

            Assert.AreEqual(2, methods.Length);
            Assert.IsFalse(methods[0].AllPreimageProperties);
            Assert.AreEqual(1, methods[0].PreimageProperties.Length);
            Assert.AreEqual("name", methods[0].PreimageProperties[0].LogicalName);

            Assert.IsTrue(methods[1].AllPreimageProperties);
        }


        public class PreimagePropertyPlugin
        {
            [Sort(1)]
            public void OnPreUpdate(Entities.Account.IAccountNameChanged nameChanged, Entities.Account.IAccountPreName preName)
            {
            }

            [Sort(2)]
            public void OnPreUpdate(Entities.Account.ICreditLimitChanged clChanged, Entities.Account preimage)
            {
            }
        }
        #endregion


        #region postimageproperty test
        [TestMethod]
        public void PostimagePropertyTest()
        {
            var methods = pluginMethodcache.ForPlugin(
                typeof(PostimagePropertyPlugin),
                (int)StepAttribute.StageEnum.Post,
                StepAttribute.MessageEnum.Update.ToString(),
                Entities.Account.EntityLogicalName,
                false);

            Assert.AreEqual(2, methods.Length);
            Assert.IsFalse(methods[0].AllPostimageProperties);
            Assert.AreEqual(1, methods[0].PostimageProperties.Length);
            Assert.AreEqual("accountnumber", methods[0].PostimageProperties[0].LogicalName);

            Assert.IsTrue(methods[1].AllPostimageProperties);
        }


        public class PostimagePropertyPlugin
        {
            [Sort(1)]
            public void OnPostUpdate(Entities.Account.IAccountNameChanged nameChanged, Entities.Account.IAccountPostAccountNumber postAccount)
            {
            }

            [Sort(2)]
            public void OnPostUpdate(Entities.Account.ICreditLimitChanged clChanged, Entities.Account postimage)
            {
            }
        }
        #endregion


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
            public void OnPreUpdate(Entities.Account.ICreditLimitChanged ac)
            {
            }

            [Sort(20)]
            public void OnPreUpdate(Entities.Account.ICreditLimitChanged ac, Entities.Account.IAccountNameChanged nc)
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
