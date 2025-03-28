﻿namespace Kipon.Xrm
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    public class BasePlugin : IPlugin
    {
        public const string Version = "1.0.10.33";
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
            var isGlobalError = true;
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var toolOrgService = serviceFactory.CreateOrganizationService(null);
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {


                using (var tracer = new Tracer(tracingService))
                {

                    var userId = context.UserId;
                    var message = context.MessageName;
                    var stage = context.Stage;
                    var isAsync = context.Mode == 1;

                    var type = message.Contains("_") ? CrmEventType.CustomPlugin : (CrmEventType)Enum.Parse(typeof(CrmEventType), context.MessageName);

                    var pluginType = this.GetType();
                    Models.Calendar.Initialize(pluginType, context.OrganizationId, toolOrgService);

                    IPluginContext pluginContext = new Services.PluginContext(this.UnsecureConfig, this.SecureConfig, context, type, userId);

                    using (var serviceCache = new Reflection.ServiceCache(context, serviceFactory, tracingService, pluginContext, this.UnsecureConfig, this.SecureConfig))
                    {
                        var entityName = context.PrimaryEntityName;

                        if (entityName == "none" || Reflection.Types.MESSAGE_WITHOUT_PRIMARY_ENTITY.Contains(message))
                        {
                            entityName = null;
                        }

                        var methods = PluginMethodCache.ForPlugin(pluginType, stage, message, entityName, context.Mode == 1);

                        var logs = new System.Collections.Generic.List<string>();

                        Implementations.InternalServiceProvider sp = null;

                        Implementations.InternalServiceProvider GetSp()
                        {
                            if (sp == null)
                            {
                                sp = new Implementations.InternalServiceProvider(serviceProvider, serviceCache, toolOrgService);
                            }
                            return sp;
                        }


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
                                var stepInitialized = false;
                                foreach (var pam in method.Parameters)
                                {
                                    nextlog += $"{comma}{pam?.FromType?.FullName}";
                                    comma = ", ";
                                    if (pam.IsInputParameter)
                                    {
                                        if (context.InputParameters.ContainsKey(pam.Name))
                                        {
                                            args[ix] = context.InputParameters[pam.Name];
                                        }
                                        else
                                        {
                                            if (pam.Name != null)
                                            {
                                                if (pam.Name.ToLower() == nameof(this.UnsecureConfig).ToLower())
                                                {
                                                    args[ix] = this.UnsecureConfig;
                                                }
                                                else
                                                if (pam.Name.ToLower() == nameof(this.SecureConfig).ToLower())
                                                {
                                                    args[ix] = this.SecureConfig;
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
                                        args[ix] = serviceCache.Resolve(pam, toolOrgService);
                                    }

                                    #region set TargetAttributes
                                    if (message == "Create" || message == "Update")
                                    {
                                        if (pam.IsMergedimage || pam.IsPreimage || pam.IsPostimage)
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
                                    if (pam.IsMergedimage || pam.IsTarget || pam.IsPreimage || pam.IsPostimage)
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
                                        if (pam.IsMergedimage)
                                        {
                                            mergedimage = (System.ComponentModel.INotifyPropertyChanged)args[ix];
                                        }

                                        if (pam.IsTarget)
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
                                    if (!stepInitialized && this is IStepInitializer stepExe)
                                    {
                                        logs.Add($"before plugin service initialiser: {nextlog}");
                                        stepExe.Initialize(GetSp());
                                        stepInitialized = true;
                                        logs.Add($"after plugin service initialiser: {nextlog}");
                                    }

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
                                catch (Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                                {
                                    inError = true;
                                    isGlobalError = false;
                                    throw;
                                }
                                catch (System.Reflection.TargetInvocationException te)
                                {
                                    inError = true;
                                    if (te.InnerException != null && te.InnerException is Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                                    {
                                        isGlobalError = false;
                                        throw te.InnerException;
                                    }

                                    if (te.InnerException != null)
                                    {
                                        isGlobalError = false;
                                        throw new InvalidPluginExecutionException(te.InnerException.Message, te.InnerException);
                                    }

                                    isGlobalError = false;
                                    throw new InvalidPluginExecutionException(te.Message, te);
                                }
                                catch (Exception be)
                                {
                                    inError = true;
                                    if (be.GetType().BaseType?.FullName == "Kipon.Xrm.Exceptions.BaseException")
                                    {
                                        // it is not a unit test, its the real thing, so we map the exception to a supported exception to allow message to parse all the way to the client
                                        isGlobalError = false;
                                        throw new InvalidPluginExecutionException($"{be.GetType().FullName}: {be.Message}", be);
                                    }

                                    isGlobalError = false;
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

                            if (this is IStepFinalizer sf)
                            {
                                logs.Add("Before step finalizer.");
                                sf.Finalize(GetSp());
                                logs.Add("After step finalizer.");
                            }
                        }
                        catch (Exception ex)
                        {
                            tracingService.Trace(ex.Message);
                            tracingService.Trace(ex.GetType().FullName);
                            tracingService.Trace(ex.StackTrace);
                            foreach (var l in logs)
                            {
                                tracingService.Trace(l);
                            }
                            isGlobalError = false;
                            throw;
                        }
                    }
                }
            }
            catch (Exception ge)
            {
                if (!isGlobalError)
                {
                    throw;
                }

                tracingService.Trace(ge.Message);
                tracingService.Trace(ge.GetType().FullName);
                tracingService.Trace(ge.StackTrace);

                if (ge is Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                {
                    throw;
                }

                throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"BasePlugin.Global: { ge.Message }", ge);
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
    }
}
