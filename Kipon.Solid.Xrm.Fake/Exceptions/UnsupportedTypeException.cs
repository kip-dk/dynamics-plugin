using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Exceptions
{
    public class UnsupportedTypeException : BaseException
    {
        public UnsupportedTypeException(string message, Type type) : base($"{message} does not support type {type.FullName}")
        {
        }
    }
}
