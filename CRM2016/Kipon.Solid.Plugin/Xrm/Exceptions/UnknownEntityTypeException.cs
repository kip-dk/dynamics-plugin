namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class UnknownEntityTypeException : BaseException
    {
        public UnknownEntityTypeException(string logicalname) : base($"{logicalname} cannot be converted to an early bound entity.")
        {
        }
    }
}
