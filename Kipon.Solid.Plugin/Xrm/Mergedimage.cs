using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm
{
    // Declarative interface to request a merged image
    // A merged image takes the tarkget of a plugin, and combine it with the field available in the target.
    // A merged image should only expose the get part of properties, because any change will not be posted 
    // back to the server.
    public interface Mergedimage<T> where T: Microsoft.Xrm.Sdk.Entity
    {
    }
}
