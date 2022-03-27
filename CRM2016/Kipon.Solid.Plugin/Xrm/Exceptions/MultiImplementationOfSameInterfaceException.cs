namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class MultiImplementationOfSameInterfaceException : BaseException
    {
        public MultiImplementationOfSameInterfaceException(Type type) : base($"{type.FullName} has more than one implementation. Mark the one to be used with export flag.")
        {
        }
    }
}
