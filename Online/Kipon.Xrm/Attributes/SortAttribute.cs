namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use the parameter on plugin methods to indicate order of execution for methods triggered by the same event
    /// The lowest value for a plugin will be used as the deployment value as well.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SortAttribute : Attribute
    {

        public SortAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }
}
