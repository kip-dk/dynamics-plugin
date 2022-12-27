using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.NugetLibTest", typeof(ICmd))]
    public class NugetLibTest : ICmd
    {
        private readonly INugetService nugetService;

        [ImportingConstructor]
        public NugetLibTest(Kipon.Xrm.Tools.ServiceAPI.INugetService nugetService)
        {
            this.nugetService = nugetService;
        }

        public Task ExecuteAsync(string[] args)
        {
            var dlls = this.nugetService.GetLibNet64();

            foreach (var dll in dlls)
            {
                Console.WriteLine($"Fandt { dll.Name } in nuget package.");
            }
            return Task.CompletedTask;
        }
    }
}
