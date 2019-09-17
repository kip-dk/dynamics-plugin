using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ImportingConstructorAttribute : Attribute
    {
    }
}
