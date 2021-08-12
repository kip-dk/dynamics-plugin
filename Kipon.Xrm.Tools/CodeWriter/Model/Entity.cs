using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    public class Entity
    {
        public string LogicalName { get; set; }
        public string ServiceName { get; set; }
        public string Primaryfield { get; set; }
        public string PrimaryfieldValuetemplate { get; set; }
        public OptionSet[] Optionsets { get; set; }
    }
}
