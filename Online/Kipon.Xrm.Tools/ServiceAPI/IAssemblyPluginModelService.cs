using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IAssemblyPluginModelService
    {
        Models.PluginAssembly Get(string name);
        void Export(string assmName, string filename);
        void Import(string filename);
    }
}
