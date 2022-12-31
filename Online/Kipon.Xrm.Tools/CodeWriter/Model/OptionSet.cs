using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    [DataContract]
    public class OptionSet
    {
        [DataMember(Name="id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name="logicalname")]
        public string Logicalname { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="multi")]
        public bool Multi { get; set; }

        [DataMember(Name="values", IsRequired = false)]
        public OptionSetValue[] Values { get; set; }
    }
}
