namespace Kipon.Xrm.Attributes
{
    using System;

    /// <summary>
    /// Property used to decorate action out parameters with needed information to push the data into the dynamics 365 ce outputparameters on the context
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
