using Kipon.Xrm.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Fake.Services;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class ServiceCacheTest
    {

        private OrganizationServiceFactory organizationServiceFactory = new OrganizationServiceFactory(null);
        private TracingService traceService = new TracingService();

        [TestMethod]
        public void ForTypeTest()
        {
            var method = typeof(TestPlugin).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)[0];
            var account = new Entities.Account { AccountId = Guid.NewGuid() };

            var context = PluginExecutionContext.ForMethodWithTarget(method, account);
            var serviceCache = new Kipon.Xrm.Reflection.ServiceCache(context, organizationServiceFactory, traceService);

            var methodCache = Kipon.Xrm.Reflection.PluginMethodCache.ForPlugin(typeof(TestPlugin), 20, "Update", Entities.Account.EntityLogicalName, false);

            var p1 = serviceCache.Resolve(methodCache[0].Parameters[0]);
            Assert.AreEqual(p1, account);

            var p2 = serviceCache.Resolve(methodCache[0].Parameters[1]) as OrganizationService;
            Assert.IsInstanceOfType(p2, typeof(OrganizationService));
            Assert.AreEqual(PluginExecutionContext.USERID, p2.UserId.Value);

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
