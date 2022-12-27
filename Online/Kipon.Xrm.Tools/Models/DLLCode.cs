using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    public class DLLCode
    {
        public string Name { get; set; }
        public byte[] Code { get; set; }


        public string Shortname => Name.Replace(".dll", string.Empty);
    }
}
