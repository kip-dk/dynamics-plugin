using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm.Attributes
{
    /// <summary>
    /// Use this attribute to decorate pre, post and merged image interface Properties to
    /// indicate that the property should also be part of the target filter. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TargetFilterAttribute : Attribute
    {
    }
}
