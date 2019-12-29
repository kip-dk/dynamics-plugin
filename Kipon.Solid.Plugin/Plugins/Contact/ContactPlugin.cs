using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Contact
{
    public class ContactPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreUpdate(Entities.Contact.INameChanged contact, Entities.Contact.IPreName prename)
        {
        }
    }
}
