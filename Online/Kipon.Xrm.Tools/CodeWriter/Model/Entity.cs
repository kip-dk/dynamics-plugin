using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    [DataContract]
    public class Entity
    {
        [DataMember(Name="logicalname", Order = 1)]
        public string LogicalName { get; set; }

        [DataMember(Name="servicename", Order = 2)]
        public string ServiceName { get; set; }

        [DataMember(Name="primaryfield", IsRequired = false, Order = 3, EmitDefaultValue =false)]
        public string Primaryfield { get; set; }

        [DataMember(Name="primaryfieldvaluetemplate", IsRequired = false, Order = 4, EmitDefaultValue = false)]
        public string PrimaryfieldValuetemplate { get; set; }

        [DataMember(Name = "Optionsets", IsRequired = false, Order = 5, EmitDefaultValue = false)]

        private OptionSet[] _optionsets;

        public OptionSet[] Optionsets 
        {
            get => this._optionsets;
            set
            {
                if (value != null && value.Length > 0)
                {
                    _optionsets = value;
                } else
                {
                    _optionsets = null;
                }
            } 
        }
    }
}
