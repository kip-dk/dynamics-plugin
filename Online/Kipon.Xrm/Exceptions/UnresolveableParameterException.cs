namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class UnresolveableParameterException : BaseException
    {
        public UnresolveableParameterException(Type type, string name) : base($"Parameter of type {type.FullName} name {name} could not be resolved to a value.")
        {
        }
    }
}
