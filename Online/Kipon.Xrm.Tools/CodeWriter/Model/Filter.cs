using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    [DataContract]
    public class Filter
    {
        [DataMember(Name = "supress-mapped-standard-optionset-properties", Order = 1)]
        public bool SupressMappedStandardOptionsetProperties { get; set; } = true;

        [DataMember(Name="entities", Order = 2)]
        public Entity[] Entities { get; set; }

        [DataMember(Name ="optionsets", Order = 3)]
        public OptionSet[] Optionsets { get; set; }

        [DataMember(Name ="actions", Order = 4)]
        public Action[] Actions { get; set; }
    }
}
