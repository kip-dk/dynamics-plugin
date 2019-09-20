using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.DI.Reflection
{
    [TestClass]
    public class TypeCacheTest
    {
        [TestMethod]
        public void ForTypeTest()
        {
            var method = typeof(PluginTemplate).GetMethod(nameof(PluginTemplate.OnPreCreate1));

            // Test target is full entity
            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]);

                // First test basic functionality for entity
                Assert.AreEqual(true, t1.IsTarget);
                Assert.AreEqual(false, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Entities.Account), t1.FromType);
                Assert.AreEqual(typeof(Entities.Account), t1.ToType);
                Assert.AreEqual(Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);

                // Test that result is cached
                var t2 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]);
                Assert.AreEqual(t2, t1);
            }

            // Test preimage is interface
            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[1]);
                Assert.AreEqual(false, t1.IsTarget);
                Assert.AreEqual(true, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Entities.IAccountPreimage), t1.FromType);
                Assert.AreEqual(typeof(Entities.Account), t1.ToType);
                Assert.AreEqual(Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }

            // Test deep target interface
            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[2]);
                Assert.AreEqual(true, t1.IsTarget);
                Assert.AreEqual(false, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Entities.IAccountNameChanged), t1.FromType);
                Assert.AreEqual(typeof(Entities.Account), t1.ToType);
                Assert.AreEqual(Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }

            // Test if two call to same entity parameters is givin same result
            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]);
                var t2 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]);
                Assert.AreEqual(t1, t2);
            }

            // Test if each entity parameter is giving different result, even though they represent same type
            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]);
                var t2 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[3]);
                Assert.AreNotEqual(t1, t2);
            }

            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[4]);
                Assert.AreEqual(typeof(Service.AccountService), t1.ToType);
                Assert.IsFalse(t1.IsTarget);
                Assert.IsFalse(t1.IsPreimage);
                Assert.IsFalse(t1.IsPostimage);
                Assert.IsFalse(t1.IsMergedimage);
                Assert.IsNull(t1.LogicalName);
                Assert.IsNotNull(t1.Constructor);
            }

            {
                var t1 = Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[5]);
                Assert.AreEqual(typeof(Entities.Account), t1.ToType);
                Assert.IsFalse(t1.IsTarget);
                Assert.IsFalse(t1.IsPreimage);
                Assert.IsFalse(t1.IsPostimage);
                Assert.IsTrue(t1.IsMergedimage);
                Assert.AreEqual(Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }
        }

        [TestMethod]
        public void ForTypeExceptionTest()
        {
            var method = typeof(PluginTemplate).GetMethod(nameof(PluginTemplate.OnPreCreate2));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.TypeMismatchException>(() => Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[0]));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnresolvableTypeException>(() => Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[1]));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnresolvableConstructorException>(() => Kipon.Xrm.DI.Reflection.TypeCache.ForParameter(method.GetParameters()[2]));
        }


        public class PluginTemplate
        {
            public void OnPreCreate1(
                Entities.Account target, 
                Entities.IAccountPreimage history, 
                Entities.IAccountNameChanged nameChanged, 
                [Kipon.Xrm.Attributes.Target]Entities.Account someName,
                ServiceAPI.IAccountService accountService,
                Entities.IAccountMergedimage merged)
            {
            }

            public void OnPreCreate2(Entities.IForgotToImplement target, ServiceAPI.INotImplementedService notImplemented, TooManyConstructors toMAny)
            {
            }
        }

        public class TooManyConstructors
        {
            public TooManyConstructors()
            {
            }

            public TooManyConstructors(string x)
            {
            }

        }
    }
}
