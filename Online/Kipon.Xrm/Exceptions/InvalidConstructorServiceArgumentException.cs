namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class InvalidConstructorServiceArgumentException : BaseException
    {
        public InvalidConstructorServiceArgumentException(System.Reflection.ConstructorInfo con, System.Reflection.ParameterInfo par) : base($"constructor on {con.DeclaringType.FullName} is requesting parameter types that are only available on plugin methods. Parse these arguments to service methods instead of constructor injection: { par.Name}")
        {
        }
    }
}
