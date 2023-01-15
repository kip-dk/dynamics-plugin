using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Deploy
{
    [Export]
    public class Plugins
    {
        private readonly ServiceAPI.IPluginDeploymentService pluginDeployService;
        private readonly ServiceAPI.IPluginAssemblyService pluginAssmService;
        private readonly ServiceAPI.IPluginTypeService pluginTypeService;
        private readonly ServiceAPI.ISdkMessageProcessingStepService sdkMessageProcessingStepService;
        private readonly ServiceAPI.ISolutionService solutionService;
        private readonly IPublishereService pubService;
        private readonly IPluginPackagesService pacService;
        private readonly INugetService nugetService;
        private readonly Kipon.Xrm.Tools.Models.Config config;

        [ImportingConstructor]
        public Plugins(
            ServiceAPI.IPluginDeploymentService pluginDeployService,
            ServiceAPI.IPluginAssemblyService pluginAssmService,
            ServiceAPI.IPluginTypeService pluginTypeService,
            ServiceAPI.ISdkMessageProcessingStepService sdkMessageProcessingStepService,
            ServiceAPI.ISolutionService solutionService,
            ServiceAPI.IPublishereService pubService,
            ServiceAPI.IPluginPackagesService pacService,
            ServiceAPI.INugetService nugetService
            )
        {
            this.pluginDeployService = pluginDeployService;
            this.pluginAssmService = pluginAssmService;
            this.pluginTypeService = pluginTypeService;
            this.sdkMessageProcessingStepService = sdkMessageProcessingStepService;
            this.solutionService = solutionService;
            this.pubService = pubService;
            this.pacService = pacService;
            this.nugetService = nugetService;
            this.config = Kipon.Xrm.Tools.Models.Config.Instance;
        }

        public void Run()
        {
            if (this.Validate())
            {
                var prefix = this.pubService.ComponentPrefix;
                var nuget = this.nugetService.GetSpec();

                var pluginPackageName = $"{ pubService.ComponentPrefix }_{ nuget.Metadata.Id }";

                var pluginPackage = this.pacService.GetPluginPackage(pluginPackageName);

                var wasCreate = false;
                if (pluginPackage == null)
                {
                    var pid = pacService.Create(this.config.Plugin.Name, pluginPackageName, nuget.Metadata.Version, System.IO.File.ReadAllBytes(this.config.Plugin.Package.Replace("$version",nuget.Metadata.Version)));
                    Console.WriteLine("PluginPackage was uploaded");

                    this.solutionService.AddMissingPluginPackage(new Xrm.Tools.Entities.PluginPackage { PluginPackageId = pid });
                    Console.WriteLine("New PluginPackage was added to solution");

                    pluginPackage = this.pacService.GetPluginPackage(pluginPackageName);
                    wasCreate = true;
                }

                var pluginAssemblies = this.pluginAssmService.ForPackage(pluginPackage.PluginPackageId.Value);
                if (pluginAssemblies.Length > 1)
                {
                    if (wasCreate)
                    {
                        this.pacService.Delete(pluginPackage.PluginPackageId.Value);
                    }

                    throw new Exception($"The framework only support one plugin assembly for now. Multi plugin support in same package will be added in the future. (2022-12-27)");
                }

                if (pluginAssemblies.Length == 0 && wasCreate)
                {
                    throw new Exception($"The package uploaded did not create any PluginAssemblies");
                }

                var dlls = nugetService.GetLibNet64();
                var plugindll = dlls.Where(r => r.Shortname == pluginAssemblies[0].Name).SingleOrDefault();

                if (plugindll == null)
                {
                    if (wasCreate)
                    {
                        this.pacService.Delete(pluginPackage.PluginPackageId.Value);
                    }
                    throw new Exception($"The nuget package did not contain the expected plugin code for library: { pluginAssemblies[0].Name }");
                }

                var dyns = new Dictionary<string, Assembly>();
                System.Reflection.Assembly pluginAssm = null;


                var others = new List<Assembly>();

                {
                    var code = System.AppDomain.CurrentDomain.Load(plugindll.Code);
                    pluginAssm = code;
                    Console.WriteLine($"Loaded: {code.FullName }");
                    dyns.Add(code.FullName, code);
                }

                foreach (var dll in dlls)
                {
                    if (dll == plugindll)
                    {
                        continue;
                    }

                    var code = System.AppDomain.CurrentDomain.Load(dll.Code);
                    others.Add(code);
                    Console.WriteLine($"Loaded: { code.FullName }");
                    dyns.Add(code.FullName, code);

                }

                if (pluginAssm == null)
                {
                    if (wasCreate)
                    {
                        this.pacService.Delete(pluginPackage.PluginPackageId.Value);
                    }
                    throw new Exception($"Plugin assembly was not loaded, it is not possible to determin needed steps to be created: { pluginAssemblies[0].Name }");
                }

                if (others.Count > 0)
                {
                    Kipon.Xrm.Reflection.TypeCache.AddServiceAssemblies(pluginAssm, others);
                }

                foreach (var type in pluginAssm.GetTypes())
                {
                    if (typeof(Kipon.Xrm.ServiceAPI.IVersion).IsAssignableFrom(type))
                    {
                        var version = (Kipon.Xrm.ServiceAPI.IVersion)Activator.CreateInstance(type);
                        if (version.No != Kipon.Xrm.Tools.Version.No)
                        {
                            throw new Exception($"Tools are version: { Kipon.Xrm.Tools.Version.No } but entities has been generated with version: { version.No }. You must regenerate entities before deploying code.");
                        }
                        break;
                    }
                }

                Assembly DynamicResolver(object sender, ResolveEventArgs args) 
                {
                    if (dyns.TryGetValue(args.Name, out Assembly asm))
                    {
                        return asm;
                    }
                    return null;
                }

                System.AppDomain.CurrentDomain.AssemblyResolve += DynamicResolver;


                // Then we find out the steps we needs according to the loaded assembly
                var upcommingPlugins = pluginDeployService.ForAssembly(pluginAssm);

                // Then we find the plugin types we already have
                var existingplugins = pluginTypeService.ForPluginAssembly(pluginAssemblies[0].PluginAssemblyId.Value);

                // Then we cleanup plugin types in CRM that are no longer needed.
                pluginTypeService.JoinAndCleanup(existingplugins, upcommingPlugins);

                // Then we find the steps we already have (after cleanup on plugins that no longer exists in the assembly)
                var steps = sdkMessageProcessingStepService.ForPluginAssembly(pluginAssemblies[0].PluginAssemblyId.Value);

                // Then we cleanup steps that are no longer needed
                steps = sdkMessageProcessingStepService.Cleanup(steps, upcommingPlugins);

                if (!wasCreate)
                {
                    pacService.Update(pluginPackage.PluginPackageId.Value, nuget.Metadata.Version, System.IO.File.ReadAllBytes(this.config.Plugin.Package.Replace("$version", nuget.Metadata.Version)));
                }

                // now map existing plugin types with existing upcomming, and create new plugintypes on the fly and map to upcomming if applicable
                pluginTypeService.FindAndJoinMissing(pluginAssemblies[0].PluginAssemblyId.Value, upcommingPlugins);

                // Create/Update steps according to upcommingPlugins defacto code, and return the brutto list of steps we have after the create/update
                steps = sdkMessageProcessingStepService.CreateOrUpdateSteps(steps, upcommingPlugins);

                // update solution with components
                this.solutionService.AddMissingPluginSteps(steps);
            }
        }


        private void RunOLD(string pluginAssemblyFileName)
        {
            // First we see if we have the plugin already, if not we update the assembly
            var pluginAssembly = pluginAssmService.FindOrCreate(pluginAssemblyFileName);

            #region validate
            var version = pluginAssmService.Assembly.GetType("Kipon.Xrm.Version")?.GetField("No").GetRawConstantValue().ToString();

            if (version != Kipon.Xrm.BasePlugin.Version)
            {
                Console.WriteLine($"Generated entities and deployment tool are not same version. Entities: {(version ?? "undefined (< 1.0.0.31)") }, Deployment tool: {Kipon.Xrm.BasePlugin.Version}");
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
            pluginTypeService.CreateAndJoinMissing(pluginAssembly.PluginAssemblyId.Value, upcommingPlugins);

            // Create/Update steps according to upcommingPlugins defacto code, and return the brutto list of steps we have after the create/update
            steps = sdkMessageProcessingStepService.CreateOrUpdateSteps(steps, upcommingPlugins);

            // update solution with components
            this.solutionService.AddMissingPluginAssembly(pluginAssembly);
            this.solutionService.AddMissingPluginSteps(steps);
        }

        private bool Validate()
        {
            if (this.config == null)
            {
                Console.WriteLine($"Please provide config file to be used on the command line on form /config:filename");
                return false;
            }

            if (string.IsNullOrEmpty(this.config.Solution))
            {
                Console.WriteLine($"Please provide solution to be used in the config file");
                return false;
            }

            return this.config.validatePluginConfiguration();
        }
    }
}
