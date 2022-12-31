using Kipon.Xrm.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Fake.Services;

namespace Kipon.Xrm.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class ServiceCacheTest : BaseTest
    {
        private OrganizationServiceFactory organizationServiceFactory = new OrganizationServiceFactory(null);

        private TracingService traceService = new TracingService();

        private Kipon.Xrm.Reflection.PluginMethod.Cache pluginMethodcache = new Kipon.Xrm.Reflection.PluginMethod.Cache(typeof(Kipon.Online.Plugin.Entities.Account).Assembly);

        [TestMethod]
        public void ForTypeTest()
        {
            var method = typeof(TestPlugin).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)[0];
            var account = new Kipon.Online.Plugin.Entities.Account { AccountId = Guid.NewGuid() };

            var unsecureConfig = "unsecure config";
            var secureConfig = "secure config";

            var context = PluginExecutionContext.ForMethodWithTarget(method, account);

            var serviceCache = new Kipon.Xrm.Reflection.ServiceCache(context, organizationServiceFactory, traceService, null,  unsecureConfig , secureConfig);

            var methodCache = pluginMethodcache.ForPlugin(typeof(TestPlugin), 20, "Update", Kipon.Online.Plugin.Entities.Account.EntityLogicalName, false);

            var p1 = serviceCache.Resolve(methodCache[0].Parameters[0]);
            Assert.AreEqual(p1, account);

            var p2 = serviceCache.Resolve(methodCache[0].Parameters[1]) as OrganizationService;
            Assert.IsInstanceOfType(p2, typeof(OrganizationService));
            Assert.AreEqual(PluginExecutionContext.USERID, p2.UserId.Value);

            var p3 = serviceCache.Resolve(methodCache[0].Parameters[2]) as Kipon.Online.Plugin.Entities.AdminCrmUnitOfWork;
            Assert.IsNotNull(p3);
            Assert.AreNotEqual(p2, p3.OrgService);

            var p4 = serviceCache.Resolve(methodCache[0].Parameters[3]) as Kipon.Online.Plugin.Service.AccountService;
            Assert.IsNotNull(p4);
            Assert.AreEqual(p2, p4.OrgService);

            var p5 = serviceCache.Resolve(methodCache[0].Parameters[4]);
            Assert.AreEqual(account, p5);

            Assert.AreEqual(unsecureConfig, p4.UnsecureConfig);
            Assert.AreEqual(secureConfig, p4.SecureConfig);
        }


        public class TestPlugin
        {
            [Sort(10)]
            public void OnPreUpdate(
                Kipon.Online.Plugin.Entities.Account target,
                Microsoft.Xrm.Sdk.IOrganizationService orgService, 
                Kipon.Online.Plugin.Entities.IAdminUnitOfWork aUow, 
                Kipon.Online.Plugin.ServiceAPI.IAccountService accountService,
                Kipon.Online.Plugin.Entities.Account.IAccountNameChanged account)
            {
            }
        }
    }
}
