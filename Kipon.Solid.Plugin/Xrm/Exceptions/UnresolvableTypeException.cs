using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class UnresolvableTypeException : Exception
    {
        public UnresolvableTypeException(Type fromType) : base($"{fromType.FullName} could not be resolved to a class with an available public constructor.")
        {
        }
    }
}
