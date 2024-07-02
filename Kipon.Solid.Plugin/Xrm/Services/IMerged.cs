using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm.Services
{
    public interface IMerged
    {
        System.Guid Id { get; }
        string LogicalName { get; }
    }
}
