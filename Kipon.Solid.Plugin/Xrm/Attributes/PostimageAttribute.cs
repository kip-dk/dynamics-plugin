using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// Use the attribute to state that the parameter should be populated with the post image
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PostimageAttribute : Attribute
    {
    }
}
