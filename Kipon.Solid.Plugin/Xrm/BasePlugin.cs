namespace Kipon.Xrm
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    public class BasePlugin : IPlugin
    {
        public const string Version = "1.0.2.0";
        public string UnsecureConfig { get; private set; }
        public string SecureConfig { get; private set; }

        internal static readonly Reflection.PluginMethod.Cache PluginMethodCache;

        static BasePlugin()
        {
            PluginMethodCache = new Reflection.PluginMethod.Cache(typeof(BasePlugin).Assembly);
            Reflection.Types.Instance.SetAssembly(typeof(BasePlugin).Assembly);
        }

        #region constructors
        public BasePlugin() : base()
        {
        }

        public BasePlugin(string unSecure, string secure) : this()
        {
            this.UnsecureConfig = unSecure;
            this.SecureConfig = secure;
        }
        #endregion

        #region iplugin impl
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var userId = context.UserId;
            var message = context.MessageName;
            var stage = context.Stage;
            var isAsync = context.Mode == 1;

            var type = message.Contains("_") ? CrmEventType.CustomPlugin : (CrmEventType)Enum.Parse(typeof(CrmEventType), context.MessageName);

            IPluginContext pluginContext = new Services.PluginContext(this.UnsecureConfig, this.SecureConfig, context, type, userId);

            using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext))
            {
                var entityName = context.PrimaryEntityName;

                if (Reflection.Types.MESSAGE_WITHOUT_PRIMARY_ENTITY.Contains(message))
                {
                    entityName = null;
                }

                var methods = PluginMethodCache.ForPlugin(this.GetType(), stage, message, entityName, context.Mode == 1);

                foreach (var method in methods)
                {
                    #region find out if method is relevant, looking a target fields
                    if (message == Attributes.StepAttribute.MessageEnum.Update.ToString() && !method.FilterAllProperties)
                    {
                        var targetEntity = (Microsoft.Xrm.Sdk.Entity)context.InputParameters["Target"];
                        if (!method.IsRelevant(targetEntity))
                        {
                            continue;
                        }
                    }
                    #endregion

                    #region now resolve all parameters
                    var args = new object[method.Parameters.Length];
                    var ix = 0;
                    System.ComponentModel.INotifyPropertyChanged mergedimage = null;
                    System.ComponentModel.INotifyPropertyChanged target = null;
                    foreach (var p in method.Parameters)
                    {
                        if (p.IsInputParameter)
                        {
                            if (context.InputParameters.ContainsKey(p.Name))
                            {
                                args[ix] = context.InputParameters[p.Name];
                            } else
                            {
                                args[ix] = null;
                            }
                        }
                        else
                        {
                            args[ix] = serviceCache.Resolve(p);
                        }

                        if (stage <= 20)
                        {
                            if (p.IsMergedimage)
                            {
                                mergedimage = (System.ComponentModel.INotifyPropertyChanged)args[ix];
                            }

                            if (p.IsTarget)
                            {
                                target = args[ix] as System.ComponentModel.INotifyPropertyChanged;
                            }
                        }
                        ix++;
                    }
                    #endregion

                    #region add mergedimage eventlistener if applicable
                    PropertyMirror mergedimageMirror = null;
                    PropertyMirror targetMirror = null;

                    if (stage <= 20)
                    {
                        if (mergedimage != null)
                        {
                            var tg = (Microsoft.Xrm.Sdk.Entity)context.InputParameters["Target"];
                            mergedimageMirror = new PropertyMirror(tg);
                            mergedimage.PropertyChanged += mergedimageMirror.MirrorpropertyChanged;
                        }

                        if (mergedimage != null && target != null)
                        {
                            targetMirror = new PropertyMirror((Microsoft.Xrm.Sdk.Entity)mergedimage);
                            target.PropertyChanged += targetMirror.MirrorpropertyChanged;
                        }
                    }
                    #endregion

                    #region run the method
                    try
                    {
                        var result = method.Invoke(this, args);

                        if (result != null && method.OutputProperties != null && method.OutputProperties.Count > 0)
                        {
                            foreach (var key in method.OutputProperties.Keys)
                            {
                                var output = method.OutputProperties[key];
                                var value = key.GetValue(result);
                                if (value != null)
                                {
                                    context.OutputParameters[output.LogicalName] = value;
                                }
                            }
                        }
                    } catch (Exception be) {
                        if (be.GetType().FullName == "Kipon.Xrm.Exceptions.BaseException") 
                        {
                            // it is not a unit test, its the real thing, so we map the exception to a supported exception to allow message to parse alle the way to the client
                            throw new InvalidPluginExecutionException($"{be.GetType().FullName}: {be.Message}", be);
                        }
                        throw;
                    } finally
                    {
                        #region cleanup mirror
                        if (stage <= 20)
                        {
                            if (mergedimageMirror != null)
                            {
                                mergedimage.PropertyChanged -= mergedimageMirror.MirrorpropertyChanged;
                                mergedimageMirror = null;
                            }

                            if (targetMirror != null)
                            {
                                target.PropertyChanged -= targetMirror.MirrorpropertyChanged;
                                targetMirror = null;
                            }
                        }
                        #endregion

                        #region prepare for next method
                        serviceCache.OnStepFinalize();
                        #endregion
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region image helper classes
        private class PropertyMirror
        {
            private Microsoft.Xrm.Sdk.Entity mirrorTo;

            internal PropertyMirror(Microsoft.Xrm.Sdk.Entity mirrorTo)
            {
                this.mirrorTo = mirrorTo; 
            }

            internal void MirrorpropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                var attr = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)sender.GetType().GetProperty(e.PropertyName).GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                if (attr != null)
                {
                    var source = sender as Microsoft.Xrm.Sdk.Entity;
                    if (source != null)
                    {
                        mirrorTo[attr.LogicalName] = source[attr.LogicalName];
                    }
                }
            } 
        }
        #endregion
    }
}
