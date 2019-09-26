namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use the attribute to state that the parameter should be populated with the post image
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PostimageAttribute : Attribute
    {
    }
}
