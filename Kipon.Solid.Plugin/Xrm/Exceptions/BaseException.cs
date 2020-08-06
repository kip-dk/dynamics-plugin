namespace Kipon.Xrm.Exceptions
{
    using System;
    [Serializable]
    public class BaseException : Exception
    {
        public BaseException(string message): base(message) { }
    }
}
