namespace Kipon.Xrm.Exceptions
{
    using System;

    public class UnresolveableParameterException : BaseException
    {
        public UnresolveableParameterException(Type type, string name) : base($"Parameter of type {type.FullName} name {name} could not be resolved to a value.")
        {
        }
    }
}
