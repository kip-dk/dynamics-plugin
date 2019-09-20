using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class UnresolvablePluginMethodException : BaseException
    {
        public UnresolvablePluginMethodException(Type type) : base($"{type.FullName} did not have any steps to be executed. Ether follow the naming convention for the plugin method, or add the [Step] custom attributes to methods to be executed")
        {

        }
    }
}
