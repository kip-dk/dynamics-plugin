using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm
{
    // Represent a post image
    // an extension should only expose getters, because any change will not be send back to crm
    public interface Postimage<T> where T: Microsoft.Xrm.Sdk.Entity
    {
    }
}
