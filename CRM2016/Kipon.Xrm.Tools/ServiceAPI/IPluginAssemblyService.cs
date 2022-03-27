using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IPluginAssemblyService
    {
        System.Reflection.Assembly Assembly { get; }
        Entities.PluginAssembly FindOrCreate(string assemblyfilename);
        void UploadAssembly();
        Entities.PluginAssembly GetPluginAssembly(string name);
    }
}
