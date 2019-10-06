using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Exceptions
{
    public class UnexpectedEventListenerException : BaseException
    {
        public UnexpectedEventListenerException(Type plugin, string message, int stage, bool isAsync = false) : base($"{plugin.FullName} does not listen for message {message} in stage {stage} {(isAsync?"Async":"")}")
        {
        }
    }
}
