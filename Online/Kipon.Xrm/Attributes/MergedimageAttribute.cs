namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use this attribute to state that the parameter should be populated with the MergedImage
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class MergedimageAttribute : Attribute
    {
    }
}
