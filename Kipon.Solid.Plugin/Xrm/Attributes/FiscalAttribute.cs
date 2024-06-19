namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use Fiscal attribute to decorate the plugin class to tell that the plugin needs the Dynamics 365 CE Fiscal year and Day start of week to be initialized from the organization service.
    /// Without this tag on the plugin, year will always start on 1. jan and first day of week will always be monday.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class FiscalAttribute : Attribute
    {
        private Type type;
        public FiscalAttribute()
        {
        }
    }
}
