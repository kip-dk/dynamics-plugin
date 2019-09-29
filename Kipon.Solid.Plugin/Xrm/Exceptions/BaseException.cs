namespace Kipon.Xrm.Exceptions
{
    using System;
    public class BaseException : Exception
    {
        public BaseException(string message): base(message) { }
    }
}
