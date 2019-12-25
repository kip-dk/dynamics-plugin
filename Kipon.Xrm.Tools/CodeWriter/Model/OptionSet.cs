using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    public class OptionSet
    {
        public string Id { get; set; }
        public string Logicalname { get; set; }
        public string Name { get; set; }
        public bool Multi { get; set; }
        public OptionSetValue[] Values { get; set; }
    }
}
