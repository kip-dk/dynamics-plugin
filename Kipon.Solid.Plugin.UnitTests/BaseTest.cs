using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests
{
    public class BaseTest
    {
        static BaseTest()
        {
            Kipon.Solid.Plugin.Setting.IsUnitTest = true;
        }
    }
}
