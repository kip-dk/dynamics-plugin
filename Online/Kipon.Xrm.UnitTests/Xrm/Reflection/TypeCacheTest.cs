using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class TypeCacheTest : BaseTest
    {
        [TestMethod]
        public void ForTypeTest()
        {
            var method = typeof(PluginTemplate).GetMethod(nameof(PluginTemplate.OnPreCreate1));

            // Test target is full entity
            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);

                // First test basic functionality for entity
                Assert.AreEqual(true, t1.IsTarget);
                Assert.AreEqual(false, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account), t1.FromType);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account), t1.ToType);
                Assert.AreEqual(Kipon.Online.Plugin.Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);

                // Test that result is cached
                var t2 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);
                Assert.AreEqual(t2, t1);
            }

            // Test preimage is interface
            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[1], null);
                Assert.AreEqual(false, t1.IsTarget);
                Assert.AreEqual(true, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.IAccountPreimage), t1.FromType);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account), t1.ToType);
                Assert.AreEqual(Kipon.Online.Plugin.Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }

            // Test deep target interface
            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[2], null);
                Assert.AreEqual(true, t1.IsTarget);
                Assert.AreEqual(false, t1.IsPreimage);
                Assert.AreEqual(false, t1.IsPostimage);
                Assert.AreEqual(false, t1.IsMergedimage);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account.IAccountNameChanged), t1.FromType);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account), t1.ToType);
                Assert.AreEqual(Kipon.Online.Plugin.Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }

            // Test if two call to same entity parameters is givin same result
            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);
                var t2 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);
                Assert.AreEqual(t1, t2);
            }

            // Test if each entity parameter is giving different result, even though they represent same type
            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);
                var t2 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[3], null);
                Assert.AreNotEqual(t1, t2);
            }

            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[4], null);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Service.AccountService), t1.ToType);
                Assert.IsFalse(t1.IsTarget);
                Assert.IsFalse(t1.IsPreimage);
                Assert.IsFalse(t1.IsPostimage);
                Assert.IsFalse(t1.IsMergedimage);
                Assert.IsNull(t1.LogicalName);
                Assert.IsNotNull(t1.Constructor);
            }

            {
                var t1 = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[5], null);
                Assert.AreEqual(typeof(Kipon.Online.Plugin.Entities.Account), t1.ToType);
                Assert.IsFalse(t1.IsTarget);
                Assert.IsFalse(t1.IsPreimage);
                Assert.IsFalse(t1.IsPostimage);
                Assert.IsTrue(t1.IsMergedimage);
                Assert.AreEqual(Kipon.Online.Plugin.Entities.Account.EntityLogicalName, t1.LogicalName);
                Assert.IsNull(t1.Constructor);
            }
        }

        [TestMethod]
        public void ForTypeExceptionTest()
        {
            var method = typeof(PluginTemplate).GetMethod(nameof(PluginTemplate.OnPreCreate2));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.TypeMismatchException>(() => Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnresolvableTypeException>(() => Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[1], null));

            Assert.ThrowsException<Kipon.Xrm.Exceptions.UnresolvableConstructorException>(() => Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[2], null));
        }


        public class PluginTemplate
        {
            public void OnPreCreate1(
                Kipon.Online.Plugin.Entities.Account target,
                Kipon.Online.Plugin.Entities.IAccountPreimage history,
                Kipon.Online.Plugin.Entities.Account.IAccountNameChanged nameChanged, 
                [Kipon.Xrm.Attributes.Target] Kipon.Online.Plugin.Entities.Account someName,
                Kipon.Online.Plugin.ServiceAPI.IAccountService accountService,
                Kipon.Online.Plugin.Entities.IAccountMergedimage merged)
            {
            }

            public void OnPreCreate2(Kipon.Online.Plugin.Entities.IForgotToImplement target, Kipon.Online.Plugin.ServiceAPI.INotImplementedService notImplemented, TooManyConstructors toMAny)
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

        #region iqueryable test
        [TestMethod]
        public void IQueryableParameterTest()
        {
            var method = typeof(QueryableParameterClass).GetMethod(nameof(QueryableParameterClass.OnPreUpdate));
            var typefor = Kipon.Xrm.Reflection.TypeCache.ForParameter(method.GetParameters()[0], null);
            Assert.IsTrue(typefor.IsQuery);
            Assert.AreEqual(typefor.RepositoryProperty.PropertyType, typeof(Kipon.Xrm.IRepository<Kipon.Online.Plugin.Entities.Contact>));
            Assert.AreEqual("GetQuery", typefor.QueryMethod.Name);
        }

        public class QueryableParameterClass
        {
            public void OnPreUpdate(System.Linq.IQueryable<Kipon.Online.Plugin.Entities.Contact> contactQuery)
            {
            }
        }
        #endregion
    }
}
