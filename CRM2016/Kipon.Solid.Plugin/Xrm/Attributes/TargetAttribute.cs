namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use this decoration to get target populated in the parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class TargetAttribute : Attribute
    {
    }
}
