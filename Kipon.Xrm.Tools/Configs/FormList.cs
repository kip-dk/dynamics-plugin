using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Configs
{
    public class FormList : List<Form>
    {
        public string Namespace { get; set; }
    }
}
