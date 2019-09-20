using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// Use Export attribute as decoration for interfaces with multi implementation to state the one and only to be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        private Type type;
        public ExportAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
