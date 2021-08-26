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
                            Type = attr.AttributeType,
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
            public Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode? Type { get; set; }
            public Control[] Controls { get; set; }

            public string TypescriptAttributeType
            {
                get
                {
                    return NameOf(this.Type, "Xrm.Attributes.", "Attribute");
                }
            }

            public string TypescriptControlType
            {
                get
                {
                    return NameOf(this.Type, "Xrm.Controls.", "Control");
                }
            }

            private string NameOf(Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode? type, string baseName, string ext)
            {
                switch (this.Type)
                {
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.BigInt:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Integer:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Decimal:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Double:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Money:
                        return $"{baseName}Number{ext}";
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Boolean:
                        if (baseName == "Xrm.Attributes.")
                        {
                            return $"{baseName}Boolean{ext}";
                        }
                        else
                        {
                            return $"{baseName}OptionSet{ext}";
                        }
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Customer:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Lookup:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Owner:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.PartyList:
                        return $"{baseName}Lookup{ext}";
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.DateTime:
                        return $"{baseName}Date{ext}";
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Memo:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.String:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Uniqueidentifier:
                        return $"{baseName}String{ext}";
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.State:
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Status:
                        return $"{baseName}OptionSet{ext}";
                    default: return $"{baseName}{ext}";
                }
            }
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
