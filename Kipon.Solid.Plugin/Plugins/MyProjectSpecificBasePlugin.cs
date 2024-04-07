using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins
{
    public abstract class MyProjectSpecificBasePlugin : Kipon.Xrm.BasePlugin, Kipon.Xrm.IStepInitializer, Kipon.Xrm.IStepFinalizer
    {
        public void Initialize(IServiceProvider serviceProvider)
        {
        }

        public void Finalize(IServiceProvider serviceProvider)
        {
        }
    }
}
