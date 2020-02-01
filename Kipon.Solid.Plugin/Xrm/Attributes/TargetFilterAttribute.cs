namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use this attribute to decorate pre, post and merged image interface Properties to
    /// indicate that the property should also be part of the target filter. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TargetFilterAttribute : Attribute
    {
    }
}
