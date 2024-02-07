namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use ImportingConstructor to decorate classes with multi public constructor, decorating the one and only constructor to be used to create the instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ImportingAttribute : Attribute
    {
        public Type Type { get; private set; }
        public ImportingAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
