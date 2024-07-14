using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Entities;
using Kipon.Solid.Plugin.ServiceAPI;
using Kipon.Xrm.Extensions.Strings;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.Service
{
    public class AccountService : ServiceAPI.IAccountService
    {
        public static string POST_MERGE_TEST = null;

        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }

        public AccountService(Microsoft.Xrm.Sdk.IOrganizationService orgService, string unsecureConfig, string secureConfig)
        {
            this.OrgService = orgService;
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
        }

        public IOrganizationService OrgService { get; private set; }

        public void OnNameChanged(Account.IAccountNameChanged target)
        {
            Kipon.Xrm.Tracer.Trace("In account service on name changed");
            if (!string.IsNullOrEmpty(target.Name))
            {
                var names = target.Name.Split(' ').Select(r => r.FirstToUpper());
                target.Name = string.Join(" ", names);
            }
            Kipon.Xrm.Tracer.Trace("In account service on name changed - finalized");
        }

        public void OnPostMerged(Entities.Account.IAccountPostMergedImage mergedimage)
        {
            POST_MERGE_TEST = PostMergedTest(mergedimage.Name, mergedimage.AccountRatingCode); 
        }

        public static string PostMergedTest(string name, Microsoft.Xrm.Sdk.OptionSetValue rating)
        {
            return $"{(name ?? "name was null")}|{(rating?.Value ?? -9999).ToString()}";

        }
    }
}
