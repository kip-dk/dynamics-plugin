namespace Kipon.Xrm.Exceptions
{
    using System;
    public class TypeMismatchException : BaseException
    {
        public TypeMismatchException(Type fromType, Type toType) : base($"{toType.FullName} does not implement expected interface {fromType.FullName}")
        {
        }
    }
}
