namespace Kipon.Xrm.Attributes
{
    using System;

    /// <summary>
    /// Property indicating that the underlying IOrganizationService must be run with system priviliges
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class AdminAttribute : Attribute
    {
    }
}
