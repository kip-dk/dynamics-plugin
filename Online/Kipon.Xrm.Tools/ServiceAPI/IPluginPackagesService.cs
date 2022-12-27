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
        Guid Create(string displayName, string uniqueName, string version, byte[] nugetpackage);
        void Update(Guid pluginPackageId, string version, byte[] nugetpackage);
        void Delete(Guid packageId);
    }
}
