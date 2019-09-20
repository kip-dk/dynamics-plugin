using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class UnknownEntityTypeException : BaseException
    {
        public UnknownEntityTypeException(string logicalname) : base($"{logicalname} cannot be converted to an early bound entity.")
        {
        }
    }
}
