using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.UploadePluginPackageTest", typeof(ICmd))]
    public class UploadePluginPackageTest : ICmd
    {
        private readonly IPluginPackagesService pacService;
        private readonly IPublishereService pubService;
        private readonly ISolutionService solService;

        [ImportingConstructor]
        public UploadePluginPackageTest(Xrm.Tools.ServiceAPI.IPluginPackagesService pacService, Xrm.Tools.ServiceAPI.IPublishereService pubService, Xrm.Tools.ServiceAPI.ISolutionService solService)
        {
            this.pacService = pacService;
            this.pubService = pubService;
            this.solService = solService;
        }

        public Task ExecuteAsync(string[] args)
        {
            var config = Kipon.Xrm.Tools.Models.Config.Instance;

            if (config == null)
            {
                Console.WriteLine("Please specify config file to be used on form: /config:filename");
                return Task.CompletedTask;
            }

            if (config.Plugin == null)
            {
                Console.WriteLine("Please specify plugin element in config file");
                return Task.CompletedTask;
            }

            var nuget = new Kipon.Xrm.Tools.Models.NugetSpec(config.Plugin.Spec);
            var packageFile = config.Plugin.Package.Replace("$version", nuget.Metadata.Version);

            if (!System.IO.File.Exists(packageFile))
            {
                Console.WriteLine($"Could not find file: { packageFile }");

                return Task.CompletedTask;
            }

            Console.WriteLine($"Ready to update file: { packageFile }");

            var name = $"{ pubService.ComponentPrefix }_{ nuget.Metadata.Id }";

            var pid = pacService.Create(config.Plugin.Name, name, nuget.Metadata.Version, System.IO.File.ReadAllBytes(packageFile));
            Console.WriteLine("Package was uploaded");

            this.solService.AddMissingPluginPackage(new Xrm.Tools.Entities.PluginPackage { PluginPackageId = pid });
            Console.WriteLine("Package was added to solution");

            return Task.CompletedTask;
        }
    }
}
