using Microsoft.Crm.Services.Utility;
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
        public static readonly string[] CUSTOMIZED = new string[]
        {
            "QualifyLead",
            "GenerateQuoteFromOpportunity"
        };

        public Activity(string messageName)
        {
            switch (messageName)
            {
                case "QualifyLead":
                    {
                        this.LogicalName = "lead";
                        this.PrimaryEntityLogicalName = "lead";
                        this.InputMembers = new Member[]
                        {
                            new Member("CreateAccount", typeof(bool), false, "CreateAccount"),
                            new Member("CreateContact", typeof(bool), false, "CreateContact"),
                            new Member("CreateOpportunity", typeof(bool), false, "CreateOpportunity"),
                            new Member("OpportunityCurrencyId", typeof(Microsoft.Xrm.Sdk.EntityReference), false, "OpportunityCurrencyId"),
                            new Member("OpportunityCustomerId", typeof(Microsoft.Xrm.Sdk.EntityReference), false, "OpportunityCustomerId"),
                            new Member("SourceCampaignId", typeof(Microsoft.Xrm.Sdk.EntityReference), false, "SourceCampaignId"),
                            new Member("Status", typeof(int), true, "Status"),
                            new Member("Target", typeof(Microsoft.Xrm.Sdk.EntityReference), false, "Target")
                        };
                        this.OutputMembers = new Member[0];
                        break;
                    }
                case "GenerateQuoteFromOpportunity":
                    {
                        this.InputMembers = new Member[]
                        {
                            new Member("OpportunityId", typeof(Guid), true, "OpportunityId"),
                            new Member("ColumnSet", typeof(Microsoft.Xrm.Sdk.Query.ColumnSet), false, "ColumnSet"),
                            new Member("ProcessInstanceId", typeof(Microsoft.Xrm.Sdk.EntityReference), false, "ProcessInstanceId")
                        };
                        this.OutputMembers = new Member[] 
                        {
                            new Member("Entity", typeof(Microsoft.Xrm.Sdk.Entity), false, "Entity"),
                        };
                        break;
                    }
                default:throw new Exceptions.ConfigurationException($"{messageName} does not have a workflow an has not been match to an action activity in the framework. If this is a std Microsoft SDK action message then please post a feature request on the git repository. Before posting and issue, please verify that the action is a valid action part of the Microsoft standard SDK");
            }
        }

        public Activity(SdkMessageWrapper message, string entityLogicalName, bool isActivityEntity)
        {
            this.PrimaryEntityLogicalName = entityLogicalName;
            this.LogicalName = entityLogicalName;

            List<Member> inputs = new List<Member>();
            List<Member> outputs = new List<Member>();

            var inputFields = message.InputFields;
            // var inputFields = message.SdkMessagePairs?.Values?.FirstOrDefault()?.Request?.RequestFields?.Values;

            if (inputFields != null)
            {
                foreach (var input in inputFields)
                {
                    var name = input.Name;

                    if (!string.IsNullOrEmpty(entityLogicalName) && ($"{entityLogicalName}id" == name.ToLower() || (isActivityEntity && name.ToLower() == "activityid")))
                    {
                        name = "Target";
                    }

                    var next = new Member(name, input.CLRFormatter.ClrToDatatype(), !input.IsOptional ?? false, name);
                    inputs.Add(next);
                }
            }

            var outputFields = message.OutputFields;
            //var outputFields = message.SdkMessagePairs?.Values?.FirstOrDefault()?.Response?.ResponseFields?.Values;

            if (outputFields != null)
            {
                foreach (var output in outputFields)
                {
                    var next = new Member(output.Name, output.CLRFormatter.ClrToDatatype(), true, output.Name);
                    outputs.Add(next);
                }
            }
            this.InputMembers = inputs.ToArray();
            this.OutputMembers = outputs.ToArray();
        }

        public Activity(string xmlDoc, string primaryEntityLogicalName)
        {
            this.PrimaryEntityLogicalName = primaryEntityLogicalName;
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

        public string PrimaryEntityLogicalName { get; set; }

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
                    if (this.Type == typeof(Guid?) && !this.Required) return "Guid?";
                    if (this.Type == typeof(Guid) && this.Required) return "Guid";
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.OptionSetValue)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.OptionSetValueCollection)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.Money)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.Entity)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.EntityReference)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.EntityCollection)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.EntityReferenceCollection)) return Type.FullName;
                    if (this.Type == typeof(Microsoft.Xrm.Sdk.Query.ColumnSet)) return Type.FullName;
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
                case "Argument(mxs:OptionSetValueCollection)": return typeof(Microsoft.Xrm.Sdk.OptionSetValueCollection);
                case "Argument(mxs:Money)": return typeof(Microsoft.Xrm.Sdk.Money);
                case "Argument(mxs:Entity)": return typeof(Microsoft.Xrm.Sdk.Entity);
                case "Argument(mxs:EntityReference)": return typeof(Microsoft.Xrm.Sdk.EntityReference);
                case "Argument(mxs:EntityCollection)": return typeof(Microsoft.Xrm.Sdk.EntityCollection);
                case "Argument(mxs:EntityReferenceCollection)": return typeof(Microsoft.Xrm.Sdk.EntityReferenceCollection);
                default: return typeof(object);
            }
        }

        public static Type ClrToDatatype(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return typeof(object);
            }
            var typePart = value.Split(',')[0];

            switch (typePart)
            {
                case "System.String": return typeof(string);
                case "System.Boolean": return typeof(bool);
                case "System.Int32": return typeof(int);
                case "System.Double": return typeof(double);
                case "System.Decimal": return typeof(decimal);
                case "System.DateTime": return typeof(DateTime);
                case "Microsoft.Xrm.Sdk.Money": return typeof(Microsoft.Xrm.Sdk.Money);
                case "Microsoft.Xrm.Sdk.OptionSetValue": return typeof(Microsoft.Xrm.Sdk.OptionSetValue);
                case "Microsoft.Xrm.Sdk.OptionSetValueCollection": return typeof(Microsoft.Xrm.Sdk.OptionSetValueCollection);
                case "Microsoft.Xrm.Sdk.EntityReference": return typeof(Microsoft.Xrm.Sdk.EntityReference);
                case "Microsoft.Xrm.Sdk.EntityReferenceCollection": return typeof(Microsoft.Xrm.Sdk.EntityReferenceCollection);
                case "Microsoft.Xrm.Sdk.Entity": return typeof(Microsoft.Xrm.Sdk.Entity);
                case "Microsoft.Xrm.Sdk.EntityCollection": return typeof(Microsoft.Xrm.Sdk.EntityCollection);
                default: return typeof(object);
            }
        }
    }
}
