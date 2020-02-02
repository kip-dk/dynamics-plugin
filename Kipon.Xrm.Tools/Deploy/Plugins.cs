using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Deploy
{
    [Export]
    public class Plugins
    {
        private ServiceAPI.IPluginDeploymentService pluginDeployService;
        private ServiceAPI.IPluginAssemblyService pluginAssmService;
        private ServiceAPI.IPluginTypeService pluginTypeService;
        private ServiceAPI.ISdkMessageProcessingStepService sdkMessageProcessingStepService;
        private ServiceAPI.ISolutionService solutionService;

        [ImportingConstructor]
        public Plugins(
            ServiceAPI.IPluginDeploymentService pluginDeployService,
            ServiceAPI.IPluginAssemblyService pluginAssmService,
            ServiceAPI.IPluginTypeService pluginTypeService,
            ServiceAPI.ISdkMessageProcessingStepService sdkMessageProcessingStepService,
            ServiceAPI.ISolutionService solutionService
            )
        {
            this.pluginDeployService = pluginDeployService;
            this.pluginAssmService = pluginAssmService;
            this.pluginTypeService = pluginTypeService;
            this.sdkMessageProcessingStepService = sdkMessageProcessingStepService;
            this.solutionService = solutionService;
        }

        public void Run(string pluginAssemblyFileName)
        {
            // First we see if we have the plugin already, if not we update the assembly
            var pluginAssembly = pluginAssmService.FindOrCreate(pluginAssemblyFileName);

            #region validate
            var version = pluginAssmService.Assembly.GetType("Kipon.Xrm.Version")?.GetField("No").GetRawConstantValue().ToString();

            if (version != Kipon.Tools.Xrm.BasePlugin.Version)
            {
                Console.WriteLine($"Generated entities and deployment tool are not same version. Entities: {(version ?? "undefined (< 1.0.0.31)") }, Deployment tool: {Kipon.Tools.Xrm.BasePlugin.Version}");
                Console.WriteLine("You should generate entities, compile and run your unittest before deploying the code to the Dynamics 365 instance, to be sure of consistant behavior.");
                return;
            }
            #endregion

            // Then we find out the steps we needs according to the loaded assembly
            var upcommingPlugins = pluginDeployService.ForAssembly(pluginAssmService.Assembly);

            // Then we find the plugin types we already have
            var existingplugins = pluginTypeService.ForPluginAssembly(pluginAssembly.PluginAssemblyId.Value);

            // Then we cleanup plugin types in CRM that are no longer needed.
            pluginTypeService.JoinAndCleanup(existingplugins, upcommingPlugins);

            // Then we find the steps we already have (after cleanup on plugins that no longer exists in the assembly)
            var steps = sdkMessageProcessingStepService.ForPluginAssembly(pluginAssembly.PluginAssemblyId.Value);

            // Then we cleanup steps that are no longer needed
            steps = sdkMessageProcessingStepService.Cleanup(steps, upcommingPlugins);

            // now we are ready to upload assembly, all types and steps that are not in the new assembly has been removed
            pluginAssmService.UploadAssembly();

            // now map existing plugin types with existing upcomming, and create new plugintypes on the fly and map to upcomming if applicable
            pluginTypeService.CreateAnJoinMissing(pluginAssembly.PluginAssemblyId.Value, upcommingPlugins);

            // Create/Update steps according to upcommingPlugins defacto code, and return the brutto list of steps we have after the create/update
            steps = sdkMessageProcessingStepService.CreateOrUpdateSteps(steps, upcommingPlugins);

            // update solution with components
            this.solutionService.AddMissingPluginAssembly(pluginAssembly);
            this.solutionService.AddMissingPluginSteps(steps);
        }
    }
}
