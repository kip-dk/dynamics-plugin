using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.SpecialService
{
    public class SpecialServicePlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Entities.kipon_multitest target, ServiceAPI.ISpecialAdminService sa)
        {
            target.kipon_name = sa.UOWImplementation;
        }
    }
}
