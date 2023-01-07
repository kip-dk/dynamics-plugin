using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class UsingNameServicePlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Entities.Account target, Kipon.Xrm.ServiceAPI.INamingService nameService)
        {
            if (Setting.IsUnitTest)
            {
                if (target.PrimaryContactId != null)
                {
                    target.Description = nameService.NameOf(target.PrimaryContactId);
                }
            }
        }
    }
}
