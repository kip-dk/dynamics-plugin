namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use the attribute to state the the parameter should be populated with the preimage
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PreimageAttribute : Attribute
    {
    }
}
