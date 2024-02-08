namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use ImportProperty to decorate 
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute()
        {
        }
    }
}
