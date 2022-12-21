using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Account
{
    public class TargetAsPreValue : Kipon.Xrm.BasePlugin
    {
        public static string EXPECTED_PRE_VALUE = "prevalue";
        public static string EXPECTED_NEW_VALUE = "newvalue";
        public void OnPostUpdate(Entities.Account.IAccountMergedImageWithTargetAttributes mergedimage)
        {
            if (mergedimage.Name == EXPECTED_NEW_VALUE)
            {
                if (mergedimage.PreName != EXPECTED_PRE_VALUE)
                {
                    throw new Exception("Unexpected value in PRE");
                }
            }
        }
    }
}
