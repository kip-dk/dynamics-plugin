using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    [DataContract]
    public class PluginAssembly
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }


        [DataMember]
        public PluginType[] PluginTypes { get; set; }

        [DataContract]
        public class PluginType
        {
            [DataMember]
            public Guid Id { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public PluginStep[] PluginSteps { get; set; }
        }


        [DataContract]
        public class PluginStep
        {
            [DataMember]
            public Guid Id { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public int Stage { get; set; }

            [DataMember]
            public string Message { get; set; }

            [DataMember]
            public int Mode { get; set; }

            [DataMember]
            public int Rank { get; set; }

            [DataMember]
            public string PrimaryEntityLogicalName { get; set; }

            [DataMember]
            public string FilteringAttributes { get; set; }

            [DataMember]
            public Image[] Images { get; set; }
        }

        [DataContract]
        public class Image
        {
            [DataMember]
            public Guid Id { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string EntityAlias { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public int ImageType { get; set; }

            [DataMember]
            public string MessagePropertyName { get; set; }

            [DataMember]
            public string Attributes1 { get; set; }
        }
    }
}
