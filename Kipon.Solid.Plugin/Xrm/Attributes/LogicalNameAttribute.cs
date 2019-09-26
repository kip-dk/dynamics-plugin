namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// For steps supporting multi entity types, decorate the method with one ore mote logical names to be supported
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class LogicalNameAttribute : Attribute
    {
        public LogicalNameAttribute(string name)
        {
            this.Value = name;
        }

        public string Value
        {
            get; private set;
        }
    }
}
