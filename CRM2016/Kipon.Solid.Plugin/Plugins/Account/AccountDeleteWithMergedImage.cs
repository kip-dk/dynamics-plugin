using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class AccountDeleteWithMergedImage : Kipon.Xrm.BasePlugin
    {
        public static string TEST_CONTENT;

        public void OnPreDelete(Entities.AccountReference target, Entities.Account.IMergedImageForDeleteTest mergedimage)
        {
            TEST_CONTENT = $"{mergedimage.AccountNumber}:{mergedimage.Name}";
        }
    }
}
