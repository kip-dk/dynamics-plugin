namespace Kipon.Xrm.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IfAttribute : Attribute
    {
        public IfAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}
