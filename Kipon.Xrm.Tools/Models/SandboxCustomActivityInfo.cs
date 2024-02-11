using Kipon.Xrm.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kipon.Xrm.Tools.Models
{
    public class SandboxCustomActivityInfo
    {
        public SandboxCustomActivityInfo()
        {
        }

        public SandboxCustomActivityInfo(System.Reflection.Assembly ass, Type type)
        {
            this.CustomActivityInfo = new CustomActivityInfoClass
            {
                AssemblyName = ass.FullName.Split(',')[0],
                AssemblyVersion = ass.FullName.Split(',')[1].Trim().Split('=')[1],
                Culture = ass.FullName.Split(',')[2].Trim().Split('=')[1],
                GroupName = ass.FullName.Split(',')[0],
                IsNet4 = "true",
                Name = type.FullName.Split('.').Last(),
                TypeName = type.FullName,
                PublicKeyToken = ass.FullName.Split(',')[3].Trim().Split('=')[1],
                PluginTypeId = Guid.NewGuid().ToString()
            };

            Inputs = new InputOutputClass
            {
                CustomActivityParameterInfo = CustomActivityParameterInfoClass.ForType(type, true)
            };

            Outputs = new InputOutputClass
            {
                CustomActivityParameterInfo = CustomActivityParameterInfoClass.ForType(type, false)
            };

            AssemblyQualifiedName = $"{type.FullName}, {ass.FullName}";
            ValidationError = new ValidatorErrorClass();
        }

        public CustomActivityInfoClass CustomActivityInfo { get; set; }
        public InputOutputClass Inputs { get; set; }
        public InputOutputClass Outputs { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public ValidatorErrorClass ValidationError { get; set; }

        public class InputOutputClass
        {
            public CustomActivityParameterInfoClass[] CustomActivityParameterInfo { get; set; }
        }

        public class CustomActivityParameterInfoClass
        {
            public CustomActivityParameterInfoClass()
            {
            }

            public static CustomActivityParameterInfoClass[] ForType(Type type, bool inputTypes)
            {
                var result = new List<CustomActivityParameterInfoClass>();

                var decorator = typeof(Microsoft.Xrm.Sdk.Workflow.InputAttribute);
                if (!inputTypes)
                {
                    decorator = typeof(Microsoft.Xrm.Sdk.Workflow.OutputAttribute);
                }

                var props = type.PublicInstanceWithDecorator(decorator);
                foreach (var prop in props)
                {
                    string name = null;
                    if (inputTypes)
                    {
                        name = ((Microsoft.Xrm.Sdk.Workflow.InputAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Workflow.InputAttribute), false).First()).Name;
                    } else
                    {
                        name = ((Microsoft.Xrm.Sdk.Workflow.OutputAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Workflow.OutputAttribute), false).First()).Name;
                    }

                    string[] entityName = new string[0];
                    string attributeName = null;

                    if (prop.PropertyFirstGenericTypeOf() == typeof(Microsoft.Xrm.Sdk.EntityReference))
                    {
                        var attrs = prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Workflow.ReferenceTargetAttribute), false).ToArray();
                        if (attrs != null && attrs.Length > 0)
                        {
                            entityName = attrs.Select(r => (Microsoft.Xrm.Sdk.Workflow.ReferenceTargetAttribute)r).Select(r => r.EntityName).ToArray();
                        }
                        else
                        {
                            throw new Exception($"Type: { type.FullName }, Property:  { prop.Name } is of type Microsoft.Xrm.Sdk.EntityReference but no Microsoft.Xrm.Sdk.Workflow.ReferenceTargetAttribute has been defined on the property");
                        }
                    }

                    if (prop.PropertyFirstGenericTypeOf() == typeof(Microsoft.Xrm.Sdk.OptionSetValue))
                    {
                        var attr = prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Workflow.AttributeTargetAttribute), false).FirstOrDefault() as Microsoft.Xrm.Sdk.Workflow.AttributeTargetAttribute;
                        if (attr != null)
                        {
                            entityName = new string[] { attr.EntityName };
                            attributeName = attr.AttributeName;
                        } else
                        {
                            throw new Exception($"Type: {type.FullName}, Property:  {prop.Name} is of type Microsoft.Xrm.Sdk.OptionSetValue but no Microsoft.Xrm.Sdk.Workflow.AttributeTargetAttribute has been defined on the property");
                        }
                    }

                    var next = new CustomActivityParameterInfoClass
                    {
                        DependencyPropertyName = prop.Name,
                        IsInOutArgument = prop.PropertyHasAllDecorators(typeof(Microsoft.Xrm.Sdk.Workflow.InputAttribute), typeof(Microsoft.Xrm.Sdk.Workflow.OutputAttribute)).ToString().ToLower(),
                        EntityNames = entityName,
                        Name = name,
                        Required = prop.PropertyHasAllDecorators(typeof(System.Activities.RequiredArgumentAttribute)).ToString().ToLower(),
                        WorkflowAttributeType = "Boolean",
                        TypeName = prop.PropertyFirstGenericTypeOf().ToSdkTypeName(),
                        IdentifierDefinition = new IdentifierDefinitionClass(),
                        AttributeName = attributeName
                    };
                    result.Add(next);
                }

                return result.ToArray();
            }

            public string Name { get; set; }
            public string TypeName { get; set; }
            public string IsInOutArgument { get; set; }
            public string WorkflowAttributeType { get; set; }
            public string Required { get; set; }
            public string AttributeName { get; set; }
            public string DependencyPropertyName { get; set; }
            public string[] EntityNames { get; set; }
            public IdentifierDefinitionClass IdentifierDefinition { get; set; }
        }


        public class CustomActivityInfoClass
        {
            public string Name { get; set; }
            public string PluginTypeId { get; set; }
            public string GroupName { get; set; }
            public string IsNet4 { get; set; }
            public string TypeName { get; set; }
            public string AssemblyName { get; set; }
            public string PublicKeyToken { get; set; }
            public string Culture { get; set; }
            public string AssemblyVersion { get; set; }
        }

        public class ValidatorErrorClass
        {
        }

        public class IdentifierDefinitionClass { }

        public string ToXml()
        {
            var ser = new XmlSerializer(this.GetType());
            using (var mem = new System.IO.MemoryStream())
            {
                ser.Serialize(mem, this);

                var result = System.Text.Encoding.UTF8.GetString(mem.ToArray())
                    .Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>")
                    .Replace("<IdentifierDefinition />", "<IdentifierDefinition xsi:nil=\"true\" />");
                return result;
            }
        }
    }
}
