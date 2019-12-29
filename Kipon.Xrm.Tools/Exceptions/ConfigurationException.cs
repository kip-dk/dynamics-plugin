using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Exceptions
{
    public class ConfigurationException : BaseException
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
