using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// Use the attribute to state the the parameter should be populated with the preimage
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PreimageAttribute : Attribute
    {
    }
}
