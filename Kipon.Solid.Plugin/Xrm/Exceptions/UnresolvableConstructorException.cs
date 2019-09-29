namespace Kipon.Xrm.Exceptions
{
    using System;
    public class UnresolvableConstructorException : BaseException
    {
        public UnresolvableConstructorException(Type type): base($"{type.FullName} has more than one constructor, mark exactly one of them with the (ImportingConstructor] attribute to indicate witch to use.)")
        {

        }
    }
}
