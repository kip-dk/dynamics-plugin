using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins
{
    public class AnunboundedactionPlugin : Kipon.Xrm.BasePlugin
    {
        public Kipon.Online.Plugin.Actions.AnunboundedactionResponse OnPrekipon_Anunboundedaction(Kipon.Online.Plugin.Actions.IAnunboundedactionRequest request)
        {
            return new Actions.AnunboundedactionResponse { Id = Guid.NewGuid().ToString() };
        }
    }
}
