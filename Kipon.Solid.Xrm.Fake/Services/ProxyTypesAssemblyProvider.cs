using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Services
{
    public class ProxyTypesAssemblyProvider : Microsoft.Xrm.Sdk.IProxyTypesAssemblyProvider
    {
        public ProxyTypesAssemblyProvider()
        {
        }

        public Assembly ProxyTypesAssembly { get; set; }
    }
}
