using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface ISdkMessageProcessingStepService
    {
        Entities.SdkMessageProcessingStep[] ForPluginAssembly(Guid pluginassemblyid);
        Entities.SdkMessageProcessingStep[] Cleanup(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins);
        Entities.SdkMessageProcessingStep[] CreateOrUpdateSteps(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins);
    }
}
