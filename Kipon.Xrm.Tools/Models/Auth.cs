using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    [DataContract]
    public class Auth
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }
    }
}
