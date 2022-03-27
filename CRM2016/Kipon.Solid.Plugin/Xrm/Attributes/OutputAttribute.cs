﻿namespace Kipon.Xrm.Attributes
{
    using System;

    /// <summary>
    /// Property indicating that the underlying IOrganizationService must be run with system priviliges
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class OutputAttribute : Attribute
    {
        public string LogicalName { get; private set; }
        public bool Required { get; private set; }

        public OutputAttribute(string logicalName, bool required)
        {
            this.LogicalName = logicalName;
            this.Required = required;
        }
    }
}
