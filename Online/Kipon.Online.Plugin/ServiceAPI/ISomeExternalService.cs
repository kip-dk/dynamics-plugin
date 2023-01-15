using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.ServiceAPI
{
    public interface ISomeExternalService
    {
        string GetNameOf(string logicalName, Guid id);
    }
}
