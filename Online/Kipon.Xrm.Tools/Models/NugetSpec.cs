using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kipon.Xrm.Tools.Models
{
    public class NugetSpec
    {
        public NugetSpec(string filename)
        {
            using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                var xml = new XmlDocument();
                xml.Load(fs);

                var metaNode = xml.SelectSingleNode("/package/metadata");
                this.Metadata = new MetadataClass
                {
                    Id = metaNode.SelectSingleNode("id")?.InnerText,
                    Version = metaNode.SelectSingleNode("version")?.InnerText,
                    Title = metaNode.SelectSingleNode("title")?.InnerText
                };
            }
        }

        public MetadataClass Metadata { get; set; }

        public class MetadataClass
        {
            public string Id { get; set; }

            public string Version { get; set; }

            public string Title { get; set; }
        }
    }
}
