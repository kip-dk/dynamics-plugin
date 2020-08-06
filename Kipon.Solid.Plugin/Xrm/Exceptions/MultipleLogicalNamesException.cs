namespace Kipon.Xrm.Exceptions
{
    using System;

    [Serializable]
    public class MultipleLogicalNamesException : BaseException
    {
        public MultipleLogicalNamesException(Type type, System.Reflection.MethodInfo method, string[] names) : base($"{ type.FullName }, method { method.Name } is requesting entities of different types. That is not allowed. {string.Join(",", names)}")
        {
        }
    }
}
