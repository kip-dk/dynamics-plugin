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
            Console.WriteLine("Starting deployment tool..." );

            this.pluginDeployer.Run();
            return Task.CompletedTask;
        }
    }
}
