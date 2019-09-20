using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class MultipleLogicalNamesException : BaseException
    {
        public MultipleLogicalNamesException(Type type, System.Reflection.MethodInfo method) : base($"{ type.FullName }, method { method.Name } is requesting entities of different types. That is not allowed.")
        {
        }
    }
}
