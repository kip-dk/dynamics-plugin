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
        Entities.SdkMessageProcessingStep[] ForSolution(string name);
        Entities.SdkMessageProcessingStepImage[] ImagesForSolution(string name);
        Entities.SdkMessageProcessingStepImage[] ImagesForPluginAssembly(Guid id);
        Entities.SdkMessageFilter[] FiltersForAssembly(Guid id);
        Microsoft.Xrm.Sdk.EntityReference GetFilterFor(Entities.SdkMessage sdkMessage, string logicalname);
    }
}
