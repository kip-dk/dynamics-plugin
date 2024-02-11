using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    public class Workflow
    {
        public SandboxCustomActivityInfo SandboxCustomActivityInfo { get; private set; }
        public Type Type { get; private set; }
        public Entities.PluginType CurrentCrmInstance { get; set; }

        public Workflow(Type type)
        {
            this.Type = type;
            this.SandboxCustomActivityInfo = new SandboxCustomActivityInfo(type.Assembly, type);
        }
    }
}
