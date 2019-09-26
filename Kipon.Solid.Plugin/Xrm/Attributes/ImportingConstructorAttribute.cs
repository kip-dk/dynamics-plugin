namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use ImportingConstructor to decorate classes with multi public constructor, decorating the one and only constructor to be used to create the instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class ImportingConstructorAttribute : Attribute
    {
    }
}
