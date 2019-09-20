using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// Use this decoration to get target populated in the parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class TargetAttribute : Attribute
    {
    }
}
