using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IPluginDeploymentService
    {
        Models.Plugin[] PluginForAssembly(System.Reflection.Assembly assembly);
        Models.Workflow[] WorkflowForAssembly(System.Reflection.Assembly assembly);
    }
}
