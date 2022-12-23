using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class SpecialAccountPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnValidateCreate(Entities.Account target, ServiceAPI.ISpecialAccountService spAccountService)
        {
            spAccountService.x();
        }
    }
}
