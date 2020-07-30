using Newtonsoft.Json;
using System;
using System.Activities.Expressions;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;

namespace Kipon.Xrm.Tools.Models
{
    public class Activity
    {
        public Activity(string xmlDoc)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlDoc);
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("a", "http://schemas.microsoft.com/netfx/2009/xaml/activities");
            nsMgr.AddNamespace("mva", "clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            nsMgr.AddNamespace("mxsw", "clr-namespace:Microsoft.Xrm.Sdk.Workflow;assembly=Microsoft.Xrm.Sdk.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            nsMgr.AddNamespace("mxswa", "clr-namespace:Microsoft.Xrm.Sdk.Workflow.Activities;assembly=Microsoft.Xrm.Sdk.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            nsMgr.AddNamespace("scg", "clr-namespace:System.Collections.Generic;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            nsMgr.AddNamespace("srs", "clr-namespace:System.Runtime.Serialization;assembly=System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            nsMgr.AddNamespace("this", "clr-namespace:");
            nsMgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            List<Member> inputs = new List<Member>();
            List<Member> outputs = new List<Member>();

            var members = doc.SelectNodes("/a:Activity/x:Members/x:Property", nsMgr);

            foreach (XmlNode member in members)
            {
                if (member.Attributes["Name"] == null || member.Attributes["Type"] == null) continue;
                var name = member.Attributes["Name"].Value;
                var type = member.Attributes["Type"].Value;

                if (name == "") continue;
                if (name == "InputEntities") continue;
                if (name == "CreatedEntities") continue;

                if (name == "Target")
                {
                    this.LogicalName = member.SelectSingleNode("x:Property.Attributes/mxsw:ArgumentEntityAttribute", nsMgr)?.Attributes["Value"]?.Value;
                    continue;
                }

                var required = member.SelectSingleNode("x:Property.Attributes/mxsw:ArgumentRequiredAttribute", nsMgr)?.Attributes["Value"]?.Value == "True";
                var logicalname = member.SelectSingleNode("x:Property.Attributes/mxsw:ArgumentEntityAttribute", nsMgr)?.Attributes["Value"]?.Value;


                var list = type.StartsWith("In") ? inputs : outputs;
                type = type.StartsWith("In") ? type.Substring(2) : type.Substring(3);

                list.Add(new Member(name, type.ToDatatype(), required, logicalname));
            }

            this.InputMembers = inputs.ToArray();
            this.OutputMembers = outputs.ToArray();
        }
        public string LogicalName { get; private set; }

        public Member[] InputMembers { get; private set; }
        public Member[] OutputMembers { get; private set; }

        public class Member
        {
            public Member(string name, Type type, bool required, string logicalname)
            {
                this.Name = name;
                this.Type = type;
                this.Required = required;
                this.LogicalName = logicalname;
            }

            public string Name { get; private set; }
            public Type Type { get; private set; }
            public bool Required { get; private set; }
            public string LogicalName { get; private set; }

            public string Typename
            {
                get
                {
                    if (this.Type == typeof(bool) && this.Required) return "bool";
                    if (this.Type == typeof(bool) && !this.Required) return "bool?";
                    if (this.Type == typeof(String)) return "string";
                    if (this.Type == typeof(int) && this.Required) return "int";
                    if (this.Type == typeof(int) && !this.Required) return "int?";
                    if (this.Type == typeof(DateTime) && this.Required) return "System.DateTime";
                    if (this.Type == typeof(DateTime) && !this.Required) return "System.DateTime?";
                    if (this.Type == typeof(double) && this.Required) return "double";
                    if (this.Type == typeof(double) && !this.Required) return "double?";
                    if (this.Type == typeof(decimal) && this.Required) return "decimal";
                    if (this.Type == typeof(decimal) && !this.Required) return "decimal?";
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.OptionSetValue) && !this.Required) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.Money)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.Entity)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.EntityReference)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.EntityCollection)) return Type.FullName;
                    return "object";
                }
            }
        }

        public bool RequireEntity(string logicalname)
        {
            if (string.IsNullOrEmpty(logicalname)) return false;
            if (this.LogicalName == logicalname) return true;
            if (this.InputMembers.Where(r => r.LogicalName == logicalname).Any()) return true;
            if (this.OutputMembers.Where(r => r.LogicalName == logicalname).Any()) return true;
            return false;
        }
    }

    public static class LocalExtensions
    {
        public static Type ToDatatype(this string value)
        {
            switch (value)
            {
                case "Argument(x:Boolean)": return typeof(bool);
                case "Argument(x:String)": return typeof(string);
                case "Argument(x:Int32)": return typeof(int);
                case "Argument(s:DateTime)": return typeof(DateTime);
                case "Argument(x:Double)": return typeof(double);
                case "Argument(x:Decimal)": return typeof(decimal);
                case "Argument(mxs:OptionSetValue)": return typeof(Microsoft.Xrm.Sdk.OptionSetValue);
                case "Argument(mxs:Money)": return typeof(Microsoft.Xrm.Sdk.Money);
                case "Argument(mxs:Entity)": return typeof(Microsoft.Xrm.Sdk.Entity);
                case "Argument(mxs:EntityReference)": return typeof(Microsoft.Xrm.Sdk.EntityReference);
                case "Argument(mxs:EntityCollection)": return typeof(Microsoft.Xrm.Sdk.EntityCollection);
                default: return typeof(object);
            }
        }
    }
}
