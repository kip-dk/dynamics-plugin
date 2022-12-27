namespace Kipon.Xrm
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    public abstract class BasePlugin : IPlugin
    {
        public const string Version = "2.0.0.1";
        internal static Reflection.PluginMethod.Cache PluginMethodCache { get; private set; }
        private static readonly System.Collections.Generic.Dictionary<Type, Config> configs = new System.Collections.Generic.Dictionary<Type, Config>();

        #region constructors
        public BasePlugin() : base()
        {
            if (PluginMethodCache == null)
            {
                PluginMethodCache = new Reflection.PluginMethod.Cache(this.GetType().Assembly);
                Reflection.Types.Instance.SetAssembly(this.GetType().Assembly);
            }

            this.Initialize();
        }

        public BasePlugin(string unSecure, string secure) : this()
        {
            if (!string.IsNullOrEmpty(unSecure) || !string.IsNullOrEmpty(secure))
            {
                configs[this.GetType()] = new Config { Secure = secure, Unsecure = unSecure };
            }
        }
        #endregion

        #region iplugin impl
        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            var toolOrgService = serviceFactory.CreateOrganizationService(null);

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var userId = context.UserId;
            var message = context.MessageName;
            var stage = context.Stage;
            var isAsync = context.Mode == 1;

            var type = message.Contains("_") ? CrmEventType.CustomPlugin : (CrmEventType)Enum.Parse(typeof(CrmEventType), context.MessageName);

            Config config = null;
            if (configs.TryGetValue(this.GetType(), out Config c))
            {
                config = c;
            }

            IPluginContext pluginContext = new Services.PluginContext(config?.Unsecure, config?.Secure, context, type, userId);

            using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext, config?.Unsecure, config?.Secure))
            {
                var entityName = context.PrimaryEntityName;

                if (entityName == "none" || Reflection.Types.MESSAGE_WITHOUT_PRIMARY_ENTITY.Contains(message))
                {
                    entityName = null;
                }

                var methods = PluginMethodCache.ForPlugin(this.GetType(), stage, message, entityName, context.Mode == 1);

                var logs = new System.Collections.Generic.List<string>();
                try
                {
                    foreach (var method in methods)
                    {
                        #region evaluate if needed - based on If attributes
                        if (method.IfAttribute != null)
                        {
                            var con = Reflection.MethodConditionEvaluater.Evaluate(method.IfAttribute, context);
                            if (!con)
                            {
                                continue;
                            }
                        }
                        #endregion

                        var nextlog = $"{method.Name}(";
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
                        var comma = "";
                        foreach (var p in method.Parameters)
                        {
                            nextlog += $"{comma}{p?.FromType?.FullName}";
                            comma = ", ";
                            if (p.IsInputParameter)
                            {
                                if (context.InputParameters.ContainsKey(p.Name))
                                {
                                    args[ix] = context.InputParameters[p.Name];
                                }
                                else
                                {
                                    if (p.Name != null)
                                    {
                                        if (p.Name.ToLower() == "unsecureconfig")
                                        {
                                            args[ix] = config?.Unsecure;
                                        }
                                        else
                                        if (p.Name.ToLower() == "secureconfig")
                                        {
                                            args[ix] = config?.Secure;
                                        }
                                        else
                                        {
                                            args[ix] = null;
                                        }
                                    }
                                    else
                                    {
                                        args[ix] = null;
                                    }
                                }
                            }
                            else
                            {
                                args[ix] = serviceCache.Resolve(p, toolOrgService);
                            }

                            #region set TargetAttributes
                            if (message == "Create" || message == "Update")
                            {
                                if (p.IsMergedimage || p.IsPreimage || p.IsPostimage)
                                {
                                    var tap = args[ix].GetType().GetProperty("TargetAttributes");

                                    if (tap != null)
                                    {
                                        var tg = (Microsoft.Xrm.Sdk.Entity)context.InputParameters["Target"];
                                        tap.SetValue(args[ix], tg.Attributes);
                                    }
                                }
                            }
                            #endregion

                            #region set preimage attributes
                            if (p.IsMergedimage || p.IsTarget || p.IsPreimage || p.IsPostimage)
                            {
                                if (message == "Update" || message == "Delete")
                                {
                                    if (context.PreEntityImages != null && context.PreEntityImages.Values != null)
                                    {
                                        var col = new AttributeCollection();
                                        foreach (Entity pre in context.PreEntityImages.Values)
                                        {
                                            foreach (var at in pre.Attributes) 
                                            {
                                                if (!col.Keys.Contains(at.Key))
                                                {
                                                    col.Add(at.Key, at.Value);
                                                }
                                            }
                                        }
                                        var tap = args[ix].GetType().GetProperty("PreimageAttributes");

                                        if (tap != null)
                                        {
                                            tap.SetValue(args[ix], col);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region add property notification to ensure mirror of set in pre state on merged images
                            if (stage <= 20 && message == "Update")
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
                            #endregion
                            ix++;
                        }
                        #endregion

                        #region add mergedimage eventlistener if applicable
                        PropertyMirror mergedimageMirror = null;
                        PropertyMirror targetMirror = null;

                        if (stage <= 20 && message == "Update")
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

                            if (target != null && target is IReplaceEntityReferenceEmptyGuidWithNull ce)
                            {
                                var entity = (Microsoft.Xrm.Sdk.Entity)target;
                                Xrm.Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ReplaceEmptyReferenceWithNull(entity, ce.ForAttributes);
                            }
                        }
                        #endregion

                        #region run the method
                        var inError = false;
                        nextlog += ")";
                        try
                        {
                            logs.Add($"before: {nextlog}");
                            var result = method.Invoke(this, args);
                            logs.Add($"after: {nextlog}");

                            if (result != null && method.OutputProperties != null && method.OutputProperties.Count > 0)
                            {
                                foreach (var key in method.OutputProperties.Keys)
                                {
                                    var output = method.OutputProperties[key];
                                    var value = key.GetValue(result);
                                    if (value != null)
                                    {
                                        #region map strongly typed entities back to base entities to allow correct serrialization back to client
                                        if (value is Microsoft.Xrm.Sdk.Entity e && value.GetType() != typeof(Microsoft.Xrm.Sdk.Entity))
                                        {
                                            var entity = new Microsoft.Xrm.Sdk.Entity(e.LogicalName, e.Id);
                                            entity.Attributes = e.Attributes;
                                            value = entity;
                                        }

                                        if (value is Microsoft.Xrm.Sdk.EntityCollection ec && ec.Entities != null && ec.Entities.Count > 0)
                                        {
                                            var final = new Microsoft.Xrm.Sdk.EntityCollection { EntityName = ec.EntityName };
                                            foreach (var ent in ec.Entities)
                                            {
                                                if (ent.GetType() == typeof(Microsoft.Xrm.Sdk.Entity))
                                                {
                                                    final.Entities.Add(ent);
                                                }
                                                else
                                                {
                                                    var entity = new Microsoft.Xrm.Sdk.Entity(ent.LogicalName, ent.Id);
                                                    entity.Attributes = ent.Attributes;
                                                    final.Entities.Add(entity);
                                                }
                                            }
                                            value = final;
                                        }
                                        #endregion

                                        context.OutputParameters[output.LogicalName] = value;
                                    }
                                }
                            }
                        }
                        catch (Kipon.Xrm.Exceptions.BaseException be)
                        {
                            inError = true;
                            throw new InvalidPluginExecutionException($"{be.GetType().FullName}: {be.Message}", be);
                        }
                        catch (Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                        {
                            inError = true;
                            throw;
                        }
                        catch (System.Reflection.TargetInvocationException te)
                        {
                            inError = true;
                            if (te.InnerException != null && te.InnerException is Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                            {
                                throw te.InnerException;
                            }

                            if (te.InnerException != null)
                            {
                                throw new InvalidPluginExecutionException(te.InnerException.Message, te.InnerException);
                            }

                            throw new InvalidPluginExecutionException(te.Message, te);
                        }
                        catch (Exception be)
                        {
                            inError = true;
                            throw new InvalidPluginExecutionException(be.Message, be);
                        }
                        finally
                        {
                            if (!inError)
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
                        }
                        #endregion
                    }
                } catch (Exception ex)
                {
                    tracingService.Trace(ex.Message);
                    tracingService.Trace(ex.GetType().FullName);
                    tracingService.Trace(ex.StackTrace);
                    foreach (var l in logs)
                    {
                        tracingService.Trace(l);
                    }
                    throw;
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
                var prop = sender.GetType().GetProperty(e.PropertyName);
                if (prop != null)
                {
                    var attr = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
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
        }
        #endregion


        #region initializer
        private static System.Collections.Generic.Dictionary<string, Kipon.Xrm.ServiceAPI.IStaticInitializer[]> initializers = new System.Collections.Generic.Dictionary<string, ServiceAPI.IStaticInitializer[]>();
        private void Initialize()
        {
            var assm = System.Reflection.Assembly.GetExecutingAssembly();
            if (initializers.ContainsKey(assm.FullName))
            {
                return;
            }

            lock (initializers)
            {
                if (initializers.ContainsKey(assm.FullName))
                {
                    return;
                }

                var result = new System.Collections.Generic.List<ServiceAPI.IStaticInitializer>();
                foreach (var type in assm.GetTypes())
                {
                    if (type is Kipon.Xrm.ServiceAPI.IStaticInitializer sc)
                    {
                        result.Add(sc);
                        sc.Initialize();
                    }
                }
                initializers[assm.FullName] = result.ToArray();
            }
        }
        #endregion

        #region classes
        internal class Config
        {
            internal string Secure { get; set; }
            internal string Unsecure { get; set; }
        }
        #endregion
    }
}
