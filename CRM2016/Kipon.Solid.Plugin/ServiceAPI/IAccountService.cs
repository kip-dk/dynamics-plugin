using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.ServiceAPI
{
    public interface IAccountService
    {
        void OnNameChanged(Entities.Account.IAccountNameChanged target);

        Microsoft.Xrm.Sdk.IOrganizationService OrgService { get; }

        void OnPostMerged(Entities.Account.IAccountPostMergedImage mergedimage);

        string UnsecureConfig { get;  }
        string SecureConfig { get; }

    }
}
