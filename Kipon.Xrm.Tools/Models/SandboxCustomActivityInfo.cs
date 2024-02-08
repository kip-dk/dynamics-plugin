using Kipon.Xrm.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                GroupName = ass.FullName.Split(',')[1].Trim().Split('=')[1],
                IsNet4 = "true",
                Name = type.FullName.Split('.').Last(),
                TypeName = type.FullName,
                PublicKeyToken = ass.FullName.Split(',')[3].Trim().Split('=')[1]
            };

            Inputs = new InputOutputClass
            {
                CustomActivityParameterInfo = CustomActivityParameterInfoClass.ForType(type, true)
            };

            Outputs = new InputOutputClass
            {
                CustomActivityParameterInfo = CustomActivityParameterInfoClass.ForType(type, false)
            };

            AssemblyQualifiedName = $"{ type.FullName }, { ass.FullName }";
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

                    string[] entityName = null;

                    if (prop.PropertyFirstGenericTypeOf() == typeof(Microsoft.Xrm.Sdk.EntityReference))
                    {
                        var attrs = prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Workflow.ReferenceTargetAttribute), false).ToArray();
                        if (attrs != null && attrs.Length > 0)
                        {
                            entityName = attrs.Select(r => (Microsoft.Xrm.Sdk.Workflow.ReferenceTargetAttribute)r).Select(r => r.EntityName).ToArray();
                        }
                    }

                    var next = new CustomActivityParameterInfoClass
                    {
                        DependencyPropertyName = prop.Name,
                        IsInOutArgument = prop.PropertyHasAllDecorators(typeof(Microsoft.Xrm.Sdk.Workflow.InputAttribute), typeof(Microsoft.Xrm.Sdk.Workflow.OutputAttribute)).ToString().ToLower(),
                        EntityNames = new EntityNamesClass
                        {
                            String = entityName != null ? entityName : null
                        },
                        Name = name,
                        Required = prop.PropertyHasAllDecorators(typeof(System.Activities.RequiredArgumentAttribute)).ToString().ToLower(),
                        WorkflowAttributeType = "Boolean",
                        TypeName = prop.PropertyFirstGenericTypeOf().ToSdkTypeName(),
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
            public string DependencyPropertyName { get; set; }
            public EntityNamesClass EntityNames { get; set; }
        }

        public class EntityNamesClass
        {
            public string[] String { get; set; }
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
    }
}
