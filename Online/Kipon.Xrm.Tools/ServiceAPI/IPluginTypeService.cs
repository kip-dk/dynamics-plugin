using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IPluginTypeService
    {
        Entities.PluginType[] ForPluginAssembly(Guid pluginAssemblyId);
        void JoinAndCleanup(Entities.PluginType[] currents, Models.Plugin[] tobees);
        void CreateAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees);
        void FindAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees);

    }
}
