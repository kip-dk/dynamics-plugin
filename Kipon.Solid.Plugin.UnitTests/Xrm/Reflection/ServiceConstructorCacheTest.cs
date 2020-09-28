using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Reflection
{
    [TestClass]
    public class ServiceConstructorCacheTest : BaseTest
    {
        [TestMethod]
        public void DefaultConstructorMethodTest()
        {
            var constructor = typeof(MyOkService).GetConstructors()[0];

            var types = Kipon.Xrm.Reflection.ServiceConstructorCache.ForConstructor(constructor);
            Assert.AreEqual(0, types.Length);
        }

        [TestMethod]
        public void SingleArgumentConstructorTest()
        {
            var constructor = typeof(MyOk2Service).GetConstructors()[0];

            var types = Kipon.Xrm.Reflection.ServiceConstructorCache.ForConstructor(constructor);
            Assert.AreEqual(1, types.Length);
        }

        [TestMethod]
        public void WrongArgumentTypeTest()
        {
            var constructor = typeof(MyNotOkService).GetConstructors()[0];
            //Assert.ThrowsException<Kipon.Xrm.Exceptions.InvalidConstructorServiceArgumentException>(() => Kipon.Xrm.Reflection.ServiceConstructorCache.ForConstructor(constructor));
        }

        public class MyOkService
        {
        }


        public class MyOk2Service
        {
            public MyOk2Service(MyOkService ok1)
            {

            }
        }

        public class MyNotOkService
        {
            public MyNotOkService(Entities.IAccountTarget tg)
            {
            }
        }

    }
}
