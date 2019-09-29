namespace Kipon.Xrm.Exceptions
{
    using System;
    public class UnresolvableTypeException : BaseException
    {
        public UnresolvableTypeException(Type fromType) : base($"{fromType.FullName} could not be resolved to a class with an available public constructor.")
        {
        }
    }
}
