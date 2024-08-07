﻿namespace Kipon.Xrm
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BaseCodeActivity : System.Activities.CodeActivity
    {
        private static readonly Dictionary<Type, System.Reflection.PropertyInfo[]> imports = new Dictionary<Type, System.Reflection.PropertyInfo[]>();
        private static readonly Dictionary<System.Reflection.PropertyInfo,  Reflection.TypeCache> typeMap = new Dictionary<System.Reflection.PropertyInfo, Reflection.TypeCache>();
        public BaseCodeActivity() : base()
        {
            Reflection.Types.Instance.SetAssembly(typeof(BasePlugin).Assembly);
        }

        protected override void Execute(CodeActivityContext executionContext)
        {
            this.InitializeAndRun(executionContext);
        }

        protected abstract void Run(CodeActivityContext executionContext);

        private void InitializeAndRun(CodeActivityContext executionContext)
        {
            var traceService = executionContext.GetExtension<ITracingService>() as ITracingService;

            using (var tracer = new Tracer(traceService))
            {
                var workflowExecutionContext = executionContext.GetExtension<IWorkflowContext>() as IWorkflowContext;
                var factory = executionContext.GetExtension<IOrganizationServiceFactory>() as IOrganizationServiceFactory;

                var type = this.GetType();

                System.Reflection.PropertyInfo[] importproperties = null;

                if (imports.TryGetValue(type, out System.Reflection.PropertyInfo[] p))
                {
                    importproperties = p;
                }
                else
                {
                    var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    var result = new List<System.Reflection.PropertyInfo>();

                    foreach (var property in properties)
                    {
                        var importAttr = property.GetCustomAttributes(typeof(Attributes.ImportAttribute), false).SingleOrDefault() as Attributes.ImportAttribute;
                        if (importAttr != null)
                        {
                            result.Add(property);
                            typeMap[property] = Reflection.TypeCache.ForProperty(property);
                        }
                    }
                    imports[type] = result.ToArray();
                    importproperties = imports[type];
                }

                if (importproperties.Length > 0)
                {
                    using (var serviceCahce = new Reflection.ServiceCache(executionContext, workflowExecutionContext, factory, traceService))
                    {
                        foreach (var prop in importproperties)
                        {
                            var typeCache = typeMap[prop];
                            var service = serviceCahce.Resolve(typeCache);
                            prop.SetValue(this, service);
                        }

                        this.Run(executionContext);
                    }
                }
            }
        }
    }
}
