namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class UnresolvableTypeException : BaseException
    {
        public UnresolvableTypeException(Type fromType, string rootSource) : base($"{fromType.FullName} could not be resolved to a class with an available public constructor. [{ rootSource }]")
        {
        }
    }
}
