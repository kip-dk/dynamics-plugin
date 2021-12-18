using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("importassmsteps", typeof(ICmd))]
    public class ImportAssmSteps : ICmd
    {
        private readonly IAssemblyPluginModelService modelService;

        [ImportingConstructor]
        public ImportAssmSteps(Kipon.Xrm.Tools.ServiceAPI.IAssemblyPluginModelService modelService)
        {
            this.modelService = modelService;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide name of file to import");
                return Task.CompletedTask; ;
            }

            this.modelService.Import(args[0]);

            return Task.CompletedTask;
        }
    }
}
