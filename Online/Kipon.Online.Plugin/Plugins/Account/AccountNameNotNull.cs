using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class AccountNameNotNull : Kipon.Xrm.BasePlugin
    {

        public static string TEST = null;
        public static string TEST_BEFORE = "test after";
        public static string TEST_AFTER = "test after";

        [Kipon.Xrm.Attributes.Filter.NotNull(nameof(Entities.Account.Name))]
        public void OnPreCreate(Entities.Account target)
        {
            TEST = TEST_AFTER;
        }

        public static void PrePareTest()
        {
            TEST = TEST_BEFORE;
        }
    }
}
