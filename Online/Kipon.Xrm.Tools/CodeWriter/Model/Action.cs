using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    [DataContract]
    public class Action
    {
        [DataMember(Name="logicalname")]
        public string LogicalName { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }
    }
}
