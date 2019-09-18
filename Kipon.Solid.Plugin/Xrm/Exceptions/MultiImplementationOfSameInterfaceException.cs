using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class MultiImplementationOfSameInterfaceException : BaseException
    {
        public MultiImplementationOfSameInterfaceException(Type type) : base($"{type.FullName} has more than one implementation. Mark the one to be used with export flag.")
        {
        }
    }
}
