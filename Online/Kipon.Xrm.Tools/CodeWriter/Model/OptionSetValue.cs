using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    [DataContract]
    public class OptionSetValue
    {
        public int Value => int.Parse(this.ValueString.Replace(".", "").Replace(",", "").Replace(" ", "").Trim());

        [DataMember(Name="name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name="value", Order = 2)]
        public string ValueString { get; set; }
    }
}
