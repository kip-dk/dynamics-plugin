namespace Kipon.Xrm.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {

        public RequiredAttribute()
        {
        }
    }
}
