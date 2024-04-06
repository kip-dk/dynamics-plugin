using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins
{
    public abstract class MyProjectSpecificBasePlugin : Kipon.Xrm.BasePlugin, Kipon.Xrm.IStepExecuter
    {
        public void Run(IServiceProvider serviceProvider, IPluginExecutionContext ctx, Action invokeAutoMappedStep)
        {
            // do whatever you need to do here, and then make sure to execute the invoceAutoMappedStep
            invokeAutoMappedStep();
        }
    }
}
