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

            Console.WriteLine("Calling deploy on Kipon.Xrm.Cmd" );

            try
            {
                this.pluginDeployer.Run(args[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error type {ex.GetType().FullName}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                var inner = ex.InnerException;
                while (inner != null)
                {
                    Console.WriteLine($"INNER: {ex.GetType().FullName}");
                    inner = inner.InnerException;
                }
            }
            return Task.CompletedTask;
        }
    }
}
