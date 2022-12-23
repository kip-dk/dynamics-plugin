using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class AccoutPostUpdate : Kipon.Xrm.BasePlugin
    {
        public const string TEST_POST_UPDATE_ACCOUNTNUMBER = "Test post update accountnumber";

        public void OnPostUpdate(Entities.Account.IAccountPostMergedImage mergedimage, ServiceAPI.IAccountService accountService)
        {
            if (mergedimage.AccountNumber == TEST_POST_UPDATE_ACCOUNTNUMBER)
            {
                accountService.OnPostMerged(mergedimage);
            }
        }
    }
}
