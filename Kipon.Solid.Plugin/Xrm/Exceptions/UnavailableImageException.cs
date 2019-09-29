namespace Kipon.Xrm.Exceptions
{
    using System;
    public class UnavailableImageException : BaseException
    {
        public UnavailableImageException(Type type, System.Reflection.MethodInfo method, string image, int stage, string message) : base($"{type.FullName}.{method.Name} is requesting image {image}. That is not supported in state {stage}, message {message}.")
        {
        }
    }
}
