using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class UnresolvableConstructorException : BaseException
    {
        public UnresolvableConstructorException(Type type): base($"{type.FullName} has more than one constructor, mark exactly one of them with the (ImportingConstructor] attribute to indicate witch to use.)")
        {

        }
    }
}
