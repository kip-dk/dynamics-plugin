using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IPluginPackagesService
    {
        Entities.PluginPackage GetPluginPackage(string name);

        Guid Create(string name, string nugetpackagefilename ,byte[] nugetpackage);
    }
}
