using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kipon.Xrm.Tools.Models
{
    public class SystemForm
    {
        public string EntityLogicalName { get; set; }
        public string Name { get; set; }

        public string FormXml { get; set; }

        public Attribute[] Attributes { get; set; }

        public Tab[] Tabs { get; set; }

        internal void Parse(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entity)
        {
            var doc = new XmlDocument();
            doc.LoadXml(this.FormXml);
            this.ParseTabs(doc);
            this.ParseAttributes(doc, entity);
        }

        private void ParseAttributes(XmlDocument doc, Microsoft.Xrm.Sdk.Metadata.EntityMetadata entity)
        {
            Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

            foreach (XmlNode xmlNode in doc.GetElementsByTagName("control"))
            {
                string controlName = xmlNode.Attributes["id"].Value.ToString().ToLower();
                var fieldName = controlName.ToLower();
                if (fieldName.StartsWith("header_") || fieldName.StartsWith("footer_"))
                {
                    fieldName = fieldName.Substring(7);
                }
                var attr = entity.Attributes.Where(r => r.LogicalName == fieldName).SingleOrDefault();
                if (attr != null)
                {
                    if (attributes.TryGetValue(fieldName, out Attribute at))
                    {
                        var nextControls = new List<Control>();
                        nextControls.AddRange(at.Controls);
                        nextControls.Add(new Control { Name = controlName });
                        at.Controls = nextControls.ToArray();
                    }
                    else
                    {
                        var next = new Attribute
                        {
                            LogicalName = attr.LogicalName,
                            DisplayName = attr.DisplayName?.UserLocalizedLabel?.Label,
                            Type = attr.AttributeTypeName?.Value,
                            Controls = new Control[] { new Control { Name = controlName } }
                        };
                        attributes.Add(fieldName, next);
                    }
                }
            }
            this.Attributes = attributes.Values.ToArray();
        }

        private void ParseTabs(XmlDocument doc)
        {
            List<Tab> tabLineList = new List<Tab>();
            foreach (XmlNode xmlNode in doc.GetElementsByTagName("tab"))
            {
                string str;
                if (xmlNode.Attributes["name"] != null)
                {
                    str = xmlNode.Attributes["name"].Value.ToString();
                }
                else
                {
                    if (xmlNode.Attributes["id"] == null)
                        throw new Exception("The tab has no name or id");
                    str = xmlNode.Attributes["id"].Value.ToString();
                }


                List<Section> stringList = new List<Section>();
                foreach (XmlNode selectNode in xmlNode.SelectNodes("descendant::section"))
                {
                    if (selectNode.Attributes["name"] != null)
                    {
                        stringList.Add(new Section { Name = selectNode.Attributes["name"].Value.ToString() });
                    }
                    else
                    {
                        stringList.Add(new Section { Name = selectNode.Attributes["id"].Value.ToString() });
                    }
                }

                Tab tabLine = new Tab ()
                {
                    Name = str,
                    FriendlyName = str,
                    Sections = stringList.ToArray()
                };
                tabLineList.Add(tabLine);
            }
            this.Tabs = tabLineList.ToArray();
        }

        public class Attribute
        {
            public string LogicalName { get; set; }
            public string DisplayName { get; set; }
            public string Type { get; set; }
            public Control[] Controls { get; set; }
        }

        public class Control
        {
            public string Name { get; set; }
        }

        public class Tab
        {
            public string Name { get; set; }
            public string FriendlyName { get; set; }
            public Section[] Sections { get; set; }
        }

        public class Section
        {
            public string Name { get; set; }
        }
    }
}
