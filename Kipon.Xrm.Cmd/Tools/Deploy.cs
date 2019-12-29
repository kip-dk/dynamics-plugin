using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("deploy", typeof(ICmd))]
    public class Deploy : ICmd
    {
        private Kipon.Xrm.Tools.Deploy.Plugins pluginDeployer;

        [ImportingConstructor]
        public Deploy(Kipon.Xrm.Tools.Deploy.Plugins pluginDeployer)
        {
            this.pluginDeployer = pluginDeployer;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide path to assembly as first parameter.");
                Console.ReadLine();
                return Task.CompletedTask;
            }

            this.pluginDeployer.Run(args[0]);

            return Task.CompletedTask;
        }
    }
}
