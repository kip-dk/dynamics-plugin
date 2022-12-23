using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class UseTargetAttributesPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Entities.Account.IUseTargetAttributes mergedimage)
        {
            mergedimage.Description = mergedimage.TelephoneChanged.ToString().ToUpper();
        }

        public void OnPreUpdate(Entities.Account.IUseTargetAttributes mergedimage)
        {
            mergedimage.Description = $"{ mergedimage.TelephoneChanged.ToString().ToUpper() } { mergedimage.PreTelephone1 }";
        }
    }
}
