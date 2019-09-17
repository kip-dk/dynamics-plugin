using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(Type fromType, Type toType) : base($"{toType.FullName} does not implement expected interface {fromType.FullName}")
        {
        }
    }
}
