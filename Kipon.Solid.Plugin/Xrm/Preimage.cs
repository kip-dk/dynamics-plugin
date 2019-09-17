using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm
{
    // Declarativ interface to represent a pre image.
    // any extension should only have get properties, because any change will not be pushed back to the server
    public interface Preimage<T> where T: Microsoft.Xrm.Sdk.Entity
    {
    }
}
