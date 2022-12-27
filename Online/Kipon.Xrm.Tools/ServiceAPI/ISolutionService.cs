using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface ISolutionService
    {
        Entities.Solution Get(string unieuqName);
        void AddMissingPluginPackage(Entities.PluginPackage package);
        void AddMissingPluginAssembly(Entities.PluginAssembly assm);
        void AddMissingPluginTypes(Entities.PluginType[] pluginTypes);
        void AddMissingPluginSteps(Entities.SdkMessageProcessingStep[] steps);
    }
}
