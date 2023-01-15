using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.ServiceAPI
{
    public interface IServiceResolverLibraries
    {
        string[] Fullnames { get; }
    }
}
