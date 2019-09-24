using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Exceptions
{
    public class CircularDependencyException : BaseException
    {
        public CircularDependencyException(string path) : base($"Circular dependendy detected. Path: {path}.")
        {
        }
    }
}
