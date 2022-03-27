namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use this attribute to decorate pre, post and merged image interface Properties to
    /// indicate that the property should also be part of the target filter. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class TargetFilterAttribute : Attribute
    {

        /// <summary>
        /// Use this constructor when decorating properties. This constructor should not be used to decorate classes
        /// </summary>
        public TargetFilterAttribute()
        {
        }

        /// <summary>
        /// Use this constructor to decorate classes (only classes that inherits Microsoft.Xrm.Sdk.Entity is relevant),
        /// and the relation between an ITarget interface and the needed target attributes. 
        /// This constructor should not be used to decorate properties.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributes"></param>
        public TargetFilterAttribute(Type type, params string[] attributes)
        {
            this.Type = type;
            this.Attributes = attributes;
        }

        public Type Type { get; private set; }
        public string[] Attributes { get; private set; }
    }
}
