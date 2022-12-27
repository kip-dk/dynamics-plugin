using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.GetPluginPackageTest", typeof(ICmd))]
    public class GetPluginPackageTest : ICmd
    {
        private readonly IPluginPackagesService pacService;

        [ImportingConstructor]
        public GetPluginPackageTest(Kipon.Xrm.Tools.ServiceAPI.IPluginPackagesService pacService)
        {
            this.pacService = pacService;
        }

        public Task ExecuteAsync(string[] args)
        {
            var pac = this.pacService.GetPluginPackage("kipon_Kipon.Online.Plugin");

            Console.WriteLine($"Package contains: { pac.Content }");

            return Task.CompletedTask;
        }
    }
}
