using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountWithConfigPlugin : Kipon.Xrm.BasePlugin
    {
        public const string EXPECT_UNSECURE = "expected unsure config string";
        public const string EXPECT_SECURE = "expected sure string";

        public AccountWithConfigPlugin()
        {
        }

        public AccountWithConfigPlugin(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        public void OnPreCreate(Entities.Account target, string unsecureConfig, string secureConfig)
        {
            if (EXPECT_SECURE != secureConfig)
            {
                throw new Exception("nope, we did not get the correct secure string");
            }

            if (unsecureConfig != EXPECT_UNSECURE)
            {
                throw new Exception("nope, we did not get the correct unsecure string");
            }
        }
    }
}
