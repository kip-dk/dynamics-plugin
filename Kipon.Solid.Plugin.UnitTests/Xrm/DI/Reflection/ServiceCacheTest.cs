using Kipon.Xrm.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.DI.Reflection
{
    [TestClass]
    public class ServiceCacheTest
    {

        private Fake.OrganizationServiceFactory organizationServiceFactory = new Fake.OrganizationServiceFactory();
        private Fake.TracingService traceService = new Fake.TracingService();

        [TestMethod]
        public void ForTypeTest()
        {
            var method = typeof(TestPlugin).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)[0];
            var account = new Entities.Account { AccountId = Guid.NewGuid() };

            var context = Fake.PluginExecutionContext.ForMethodWithTarget(method, account);
            var serviceCache = new Kipon.Xrm.DI.Reflection.ServiceCache(context, organizationServiceFactory, traceService);

            var methodCache = Kipon.Xrm.DI.Reflection.PluginMethodCache.ForPlugin(typeof(TestPlugin), 20, "Update", Entities.Account.EntityLogicalName, false);

            var p1 = serviceCache.Resolve(methodCache[0].Parameters[0]);
            Assert.AreEqual(p1, account);

            var p2 = serviceCache.Resolve(methodCache[0].Parameters[1]) as Fake.OrganizationService;
            Assert.IsInstanceOfType(p2, typeof(Fake.OrganizationService));
            Assert.AreEqual(Fake.PluginExecutionContext.USERID, p2.UserId.Value);

            var p3 = serviceCache.Resolve(methodCache[0].Parameters[2]) as Kipon.Solid.Plugin.Entities.AdminCrmUnitOfWork;
            Assert.IsNotNull(p3);
            Assert.AreNotEqual(p2, p3.OrgService);

            var p4 = serviceCache.Resolve(methodCache[0].Parameters[3]) as Kipon.Solid.Plugin.Service.AccountService;
            Assert.IsNotNull(p4);
            Assert.AreEqual(p2, p4.OrgService);

            var p5 = serviceCache.Resolve(methodCache[0].Parameters[4]);
            Assert.AreEqual(account, p5);
        }


        public class TestPlugin
        {
            [Sort(10)]
            public void OnPreUpdate(
                Entities.Account target,
                Microsoft.Xrm.Sdk.IOrganizationService orgService, 
                Kipon.Solid.Plugin.Entities.IAdminUnitOfWork aUow, 
                Kipon.Solid.Plugin.ServiceAPI.IAccountService accountService,
                Kipon.Solid.Plugin.Entities.IAccountNameChanged account)
            {
            }
        }
    }
}
