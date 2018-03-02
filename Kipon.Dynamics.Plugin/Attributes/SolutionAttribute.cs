using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SolutionAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
