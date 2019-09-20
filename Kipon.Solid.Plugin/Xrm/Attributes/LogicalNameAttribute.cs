using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// For steps supporting multi entity types, decorate the method with one ore mote logical names to be supported
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class LogicalNameAttribute : Attribute
    {
        public LogicalNameAttribute(string name)
        {
            this.Value = name;
        }

        public string Value
        {
            get; private set;
        }
    }
}
