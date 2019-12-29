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

        [ImportingConstructor]
        public Plugins(ServiceAPI.IPluginDeploymentService pluginDeployService)
        {
            this.pluginDeployService = pluginDeployService;
        }

        public void Run(string pluginAssemblyFileName)
        {
            var assm = System.Reflection.Assembly.LoadFrom(pluginAssemblyFileName);
            var plugins = pluginDeployService.ForAssembly(assm);

        }
    }
}
