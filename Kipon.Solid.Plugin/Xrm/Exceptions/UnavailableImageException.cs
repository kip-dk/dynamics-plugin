using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class UnavailableImageException : BaseException
    {
        public UnavailableImageException(Type type, System.Reflection.MethodInfo method, string image, int stage, string message) : base($"{type.FullName}.{method.Name} is requesting image {image}. That is not supported in state {stage}, message {message}.")
        {
        }
    }
}
