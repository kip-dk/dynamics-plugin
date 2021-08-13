using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Configs
{
    public class DeployList : List<Deploy>
    {
        public string Solution { get; set; }
    }
}
