namespace Kipon.Xrm
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class BaseCodeActivity : System.Activities.CodeActivity
    {
        private static readonly Dictionary<Type, System.Reflection.PropertyInfo[]> imports = new Dictionary<Type, System.Reflection.PropertyInfo[]>();
        private static readonly Dictionary<PropertyInfo, Type> propertyTypes = new Dictionary<PropertyInfo, Type>();
        protected override void Execute(CodeActivityContext executionContext)
        {
            this.Initialize(executionContext);
            this.Run();
        }

        protected abstract void Run();

        private void Initialize(CodeActivityContext executionContext)
        {
            var traceService = executionContext.GetExtension<ITracingService>() as ITracingService;
            var workflowExecutionContext = executionContext.GetExtension<IWorkflowContext>()as IWorkflowContext;
            var factory = executionContext.GetExtension<IOrganizationServiceFactory>() as IOrganizationServiceFactory;
            var orgService = factory.CreateOrganizationService(workflowExecutionContext.UserId) as IOrganizationService;
            var orgAdminService = factory.CreateOrganizationService(null) as IOrganizationService;

            var type = this.GetType();

            System.Reflection.PropertyInfo[] importproperties = null;

            if (imports.TryGetValue(type, out System.Reflection.PropertyInfo[] p))
            {
                importproperties = p;
            }
            else
            {
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public & System.Reflection.BindingFlags.Instance);
                var result = new List<System.Reflection.PropertyInfo>();


                foreach (var prop in properties)
                {
                    var importAttr = (Attributes.ImportingAttribute)prop.GetCustomAttributes(typeof(Attributes.ImportingAttribute), false).SingleOrDefault();
                    if (importAttr != null)
                    {
                        result.Add(prop);
                        propertyTypes[prop] = importAttr.Type;
                    }
                }
                importproperties = imports[type] = result.ToArray();
            }

            foreach (var prop in importproperties)
            {
            }
        }
    }
}
