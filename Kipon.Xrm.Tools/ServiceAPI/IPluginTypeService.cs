using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IPluginTypeService
    {
        Entities.PluginType[] GetPluginTypes(Guid pluginAssemblyId);
        void JoinAndCleanupPlugins(Entities.PluginType[] currents, Models.Plugin[] tobee);
        void CreateAnJoinMissingPlugins(Guid pluginassemblyId, Models.Plugin[] tobee);

        Entities.PluginType[] GetWorkflowTypes(Guid pluginAssemblyId);
        void JoinAndCleanupWorkflows(Entities.PluginType[] currents, Models.Workflow[] tobee);
        void CreateAnJoinMissingWorkflows(Guid pluginassemblyId, Models.Workflow[] tobee);
    }
}
