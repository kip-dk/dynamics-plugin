using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    /// <summary>
    /// Use ImportingConstructor to decorate classes with multi public constructor, decorating the one and only constructor to be used to create the instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class ImportingConstructorAttribute : Attribute
    {
    }
}
