using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.UnitTests
{
    public class BaseTest
    {
        static BaseTest() 
        {
            var types = Kipon.Xrm.Reflection.Types.Instance;
            types.SetAssembly(typeof(Kipon.Online.Plugin.Entities.IUnitOfWork).Assembly);
        }
    }
}
