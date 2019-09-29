namespace Kipon.Xrm.Exceptions
{
    public class CircularDependencyException : BaseException
    {
        public CircularDependencyException(string path) : base($"Circular dependendy detected. Path: {path}.")
        {
        }
    }
}
