namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class CircularDependencyException : BaseException
    {
        public CircularDependencyException(string path) : base($"Circular dependendy detected. Path: {path}.")
        {
        }
    }
}
