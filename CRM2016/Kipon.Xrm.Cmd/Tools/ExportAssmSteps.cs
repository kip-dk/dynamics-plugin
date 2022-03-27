using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("exportassmsteps", typeof(ICmd))]
    public class ExportAssmSteps : ICmd
    {
        private readonly IAssemblyPluginModelService modelService;

        [ImportingConstructor]
        public ExportAssmSteps(Kipon.Xrm.Tools.ServiceAPI.IAssemblyPluginModelService modelService)
        {
            this.modelService = modelService;
        }


        public Task ExecuteAsync(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Please provide name of assm and name of output file");
                return Task.CompletedTask;
            }

            var file = args[1];
            if (!file.EndsWith(".json"))
            {
                file += ".json";
            }
            this.modelService.Export(args[0], file);
            return Task.CompletedTask;
        }
    }
}
