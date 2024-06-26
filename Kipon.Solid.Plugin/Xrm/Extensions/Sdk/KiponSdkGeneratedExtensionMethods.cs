﻿namespace Kipon.Xrm.Extensions.Sdk
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk;
    using Reflection;

    public static partial class KiponSdkGeneratedExtensionMethods
    {
        private static readonly Dictionary<string, Type> entittypes = new Dictionary<string, Type>();
        private static readonly Dictionary<string, System.Reflection.MethodInfo> TO_ENT_GENS = new Dictionary<string, System.Reflection.MethodInfo>();
        private static readonly System.Reflection.MethodInfo TO_ENTITY = typeof(Microsoft.Xrm.Sdk.Entity).GetMethod("ToEntity", new Type[0]);
        private static readonly Dictionary<string, string[]> targetattributes = new Dictionary<string, string[]>();

        public const string UNDEFINED_ATTRIBUTE_MESSAGE = "Entity of type {0} does not have a property named {1}";
        public const string MISSING_DECORATION_ATTRIBUTE_MESSAGE = "Property {0} on entity {1} is not decorated with Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";

        private static readonly string[] CLONE_EXCLUDE = new string[]
        {
            "createdon",
            "createdby",
            "modifiedon",
            "modifiedby",
            "createdonbehalfby",
            "modifiedonbehalfby",
            "importsequencenumber"
        };

        /// <summary>
        /// Converts the entity to its early bound entity if it is not already such implemention. In that case it returns itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ent"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static T ToEarlyBoundEntity<T>(this T ent) where T : Microsoft.Xrm.Sdk.Entity
        {
            if (ent.GetType().BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
            {
                return ent;
            }

            if (TO_ENT_GENS.ContainsKey(ent.LogicalName))
            {
                return TO_ENT_GENS[ent.LogicalName].Invoke(ent, new object[0]) as T;
            }

            if (!entittypes.ContainsKey(ent.LogicalName))
            {
                var enttype = Reflection.Types.Instance.EntityTypeFor(ent.LogicalName);
                if (enttype != null)
                {
                    entittypes[ent.LogicalName] = enttype;
                    return ToEarlyBoundEntity(ent);
                }
                throw new Exceptions.UnknownEntityTypeException(ent.LogicalName);
            }

            var type = entittypes[ent.LogicalName];
            TO_ENT_GENS[ent.LogicalName] = TO_ENTITY.MakeGenericMethod(type);

            return TO_ENT_GENS[ent.LogicalName].Invoke(ent, new object[0]) as T;
        }


        /// <summary>
        /// Returns the target filter attributes of the entity, when implementing interfaceType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static string[] TargetFilterAttributesOf<T>(this T entity, Type interfaceType) where T : Microsoft.Xrm.Sdk.Entity
        {
            return typeof(T).TargetFilterAttributesOf(interfaceType);
        }


        /// <summary>
        /// Ths method resolved the attributes that are related to  specific interface, that inherits from Kipon.Xrm.ITarget
        /// </summary>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="interfaceType">The interface implemented</param>
        /// <returns>List of fields that are part of target of the interface</returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static string[] TargetFilterAttributesOf(this Type entityType, Type interfaceType)
        {
            if (!typeof(Microsoft.Xrm.Sdk.Entity).IsAssignableFrom(entityType))
            {
                throw new InvalidPluginExecutionException("Only types that inherits from Microsoft.Xrm.Sdk.Entity can have class based TargetFilterAttribute.");
            }

            var key = $"{interfaceType.FullName}|{entityType.Name.ToLower()}";

            {
                if (targetattributes.TryGetValue(key, out string[] v))
                {
                    return v;
                }
            }

            lock (targetattributes)
            {
                if (targetattributes.TryGetValue(key, out string[] v))
                {
                    return v;
                }

                var type = entityType.GetTargetFilterAttribute(interfaceType);

                if (type == null)
                {
                    targetattributes[key] = null;
                    return null;
                }

                targetattributes[key] = type.Attributes;
                return targetattributes[key];
            }

        }

        /// <summary>
        /// Returns the property value of an entity if it is in there as an object, otherwise null
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="attribLogicalName">Name of attribute</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static object GetSafeValue(this Microsoft.Xrm.Sdk.Entity entity, string attribLogicalName)
        {
            if (!entity.Attributes.ContainsKey(attribLogicalName))
            {
                return null;
            }
            return entity[attribLogicalName];
        }


        /// <summary>
        /// Return the target of the parent event. We can only find target for Create and Update parent events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns
        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ParentTarget<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message, Guid id) where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            if (message != "Create" && message != "Update")
            {
                throw new Exceptions.BaseException("ParentTarget method only support search for Create and Update request");
            }

            var parent = ctx.ParentContext;
            if (parent == null)
            {
                return null;
            }

            var proto = new T();

            if (parent.MessageName == message && parent.PrimaryEntityName == proto.LogicalName && parent.PrimaryEntityId == id)
            {
                var result = (Microsoft.Xrm.Sdk.Entity)parent.InputParameters["Target"];
                return result.ToEntity<T>();
            }

            if (ctx.MessageName == "ExecuteTransaction")
            {
                if (ctx.InputParameters.Contains("Requests"))
                {
                    var requests = ctx.InputParameters["Requests"] as Microsoft.Xrm.Sdk.OrganizationRequestCollection;
                    if (requests != null)
                    {
                        foreach (var r in requests)
                        {
                            switch (message)
                            {
                                case "Create":
                                    {
                                        if (r is Microsoft.Xrm.Sdk.Messages.CreateRequest c)
                                        {
                                            if (c.Target.LogicalName == proto.LogicalName && c.Target.Id == id)
                                            {
                                                return c.Target.ToEntity<T>();
                                            }
                                        }
                                        break;
                                    }
                                case "Update":
                                    {
                                        if (r is Microsoft.Xrm.Sdk.Messages.UpdateRequest c)
                                        {
                                            if (c.Target.LogicalName == proto.LogicalName && c.Target.Id == id)
                                            {
                                                return c.Target.ToEntity<T>();
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return parent.ParentTarget<T>(message, id);
        }


        /// <summary>
        /// Find out if the ctx is OR has a parent contect that match the filter
        /// </summary>
        /// <param name="ctx">The context</param>
        /// <param name="message">The message, ex Create, Update ..., required</param>
        /// <param name="entityLogicalName">The logical name of the key, if null any, optional</param>
        /// <param name="id">The id of the entity expected to be a parent event, if null any, optional</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static bool IsChildOf(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message, string entityLogicalName = null, Guid? id = null)
        {
            if (ctx == null)
            {
                return false;
            }

            if (ctx.MessageName == message && (entityLogicalName == null || ctx.PrimaryEntityName == entityLogicalName) && (id == null || ctx.PrimaryEntityId == id))
            {
                return true;
            }

            if (ctx.MessageName == "ExecuteTransaction")
            {
                if (ctx.InputParameters.Contains("Requests"))
                {
                    try
                    {
                        var requests = ctx.InputParameters["Requests"] as Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.OrganizationRequest>;
                        if (requests != null)
                        {
                            foreach (var r in requests)
                            {
                                try
                                {
                                    switch (message)
                                    {
                                        case "Create":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.CreateRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                        case "Update":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.UpdateRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                        case "Delete":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.DeleteRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                    }

                                    if (r.RequestName == message)
                                    {
                                        if (entityLogicalName == null && id == null)
                                        {
                                            return true;
                                        }

                                        if (r.Parameters.ContainsKey("Target") && r.Parameters["Target"] is Microsoft.Xrm.Sdk.EntityReference targetid)
                                        {
                                            if (targetid.LogicalName == entityLogicalName && (id == null || id == targetid.Id))
                                            {
                                                return true;
                                            }
                                        }

                                        if (r.Parameters.ContainsKey("Target") && r.Parameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
                                        {
                                            if (target.LogicalName == entityLogicalName && (id == null || id == target.Id))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == null || !ex.Message.Contains("contains data from a type that maps to the name"))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == null || !ex.Message.Contains("contains data from a type that maps to the name"))
                        {
                            throw;
                        }
                    }
                }
            }
            return ctx.ParentContext.IsChildOf(message, entityLogicalName, id);
        }

        /// <summary>
        /// Returns true if the attribute is part of the target payload of the request. It only make sense in Create, Update requests
        /// </summary>
        /// <param name="ctx">The plugin execution context</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>True if context contains Target as an entity and the entity has the attribute set.</returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool AttributeChanged(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attributeName)
        {
            if (ctx.InputParameters.Contains("Target") && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity e)
            {
                return e.Attributes.ContainsKey(attributeName.ToLower());
            }
            return false;
        }

        /// <summary>
        ///  Create an in memory instance as a clone of target.
        /// </summary>
        /// <typeparam name="T">Any strongly typed entity</typeparam>
        /// <param name="target">The entity instance</param>
        /// <param name="ommits">Fields that should be omitted in the copy</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static T Clone<T>(this T target, params string[] ommits) where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            var result = new T();

            var omitsLower = ommits?.Select(r => r.ToLower()).ToArray();
            var logicalname = target.LogicalName.ToLower();
            var keyname = $"{logicalname}id";

            switch (logicalname)
            {
                case "email":
                case "phonecall":
                case "appointment":
                case "task":
                case "campaignresponse":
                case "fax":
                case "letter":
                case "campaignactivity":
                case "socialactivity":
                case "opportunityclose":
                case "quoteclose":
                case "orderclose":
                case "incidentresolution":
                case "serviceappointment":
                case "recurringappointmentmaster":
                case "untrackedemail":
                case "bulkoperation":
                    {
                        keyname = "activityid";
                        break;
                    }
            }

            if (target.Attributes != null)
            {
                foreach (var k in target.Attributes.Keys)
                {
                    if (CLONE_EXCLUDE.Contains(k)) continue;
                    if (omitsLower != null && omitsLower.Contains(k)) continue;
                    if (k == keyname) continue;

                    result[k] = target[k];
                }
            }
            return result;
        }


        /// <summary>
        /// Convinient method to find the prevalue of something in the execution context.
        /// </summary>
        /// <typeparam name="T">The type T you expect to get back</typeparam>
        /// <param name="ctx">The Dynamics 365 execution context</param>
        /// <param name="entitypropertyname">The property name on the strongly typed entity of the flow</param>
        /// <returns></returns>
        // [System.Diagnostics.DebuggerNonUserCode()]

        public static T PreValueOf<T>(this IPluginExecutionContext ctx, string entitypropertyname)
        {
            System.Reflection.PropertyInfo prop = null;
            System.Type propertyType = null;

            string attributeName = null;
            if (ctx.PreEntityImages != null)
            {
                var lower = entitypropertyname.ToLower();

                foreach (var pi in ctx.PreEntityImages.Values)
                {
                    if (pi.Attributes.ContainsKey(lower))
                    {
                        var v = pi[lower];
                        if (v == null)
                        {
                            return default(T);
                        }
                        return v.ToTValueType<T>();
                    }

                    if (prop == null)
                    {
                        var e = pi.ToEarlyBoundEntity();
                        var t = e.GetType();

                        prop = t.GetProperty(entitypropertyname);

                        if (prop == null)
                        {
                            foreach (var p in t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                            {
                                var c = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)p.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                                if (c != null && c.LogicalName == entitypropertyname)
                                {
                                    prop = p;
                                    break;
                                }
                            }
                        }

                        if (prop == null)
                        {
                            throw new InvalidPluginExecutionException(string.Format(UNDEFINED_ATTRIBUTE_MESSAGE, pi.LogicalName, entitypropertyname));
                        }

                        var ca = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                        if (ca == null)
                        {
                            throw new InvalidPluginExecutionException(string.Format(MISSING_DECORATION_ATTRIBUTE_MESSAGE, pi.LogicalName, entitypropertyname));
                        }

                        attributeName = ca.LogicalName;

                        propertyType = prop.PropertyType;

                        if (propertyType.IsGenericType)
                        {
                            var under = propertyType.GetGenericTypeDefinition();
                            if (under == typeof(Nullable<>))
                            {
                                propertyType = Nullable.GetUnderlyingType(propertyType);
                            }
                        }
                    }

                    if (pi.Attributes.Contains(attributeName))
                    {
                        var v = pi[attributeName];

                        if (v == null)
                        {
                            return default(T);
                        }

                        return v.ToTValueType<T>();
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Return input parameters of the first parent object matching logicalname and message as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="logicalName">name of entity, null if unbound action</param>
        /// <param name="message">the message name, ex. update, create, or the action name</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ParentInputParameters<T>(this IPluginExecutionContext ctx, string logicalName, string message)
        {
            var parent = ctx.ParentContext(logicalName, message);
            if (parent != null)
            {
                var type = typeof(T);
                var cType = type;
                #region T is an interface, and we need to find the one and only implementation
                if (type.IsInterface)
                {
                    cType = Types.Instance.TypeForInterface(type);
                }
                #endregion

                #region the T knows about IPluginExecutionContext and can extract values itself
                var con = cType.GetConstructor(new Type[] { typeof(IPluginExecutionContext) });
                if (con != null)
                {
                    return (T)con.Invoke(new object[] { parent });
                }
                #endregion

                #region parameter less constructor on type, simply assign value by naming convention
                con = cType.GetConstructor(new Type[0]);
                if (con != null)
                {
                    var result = con.Invoke(null);

                    foreach (var pam in parent.InputParameters)
                    {
                        if (pam.Value != null)
                        {
                            var name = pam.Key;
                            var prop = type.GetProperty(pam.Key);
                            if (prop != null && prop.CanWrite && prop.PropertyType.IsAssignableFrom(pam.Value.GetType()))
                            {
                                prop.SetValue(result, pam.Value);
                            }
                        }
                    }
                    return (T)result;
                }
                #endregion
            }
            return default(T);
        }

        /// <summary>
        /// Returns a single parameters as type T or default(T) if parent not found or parameter name not contained in parent context input parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="logicalName"></param>
        /// <param name="message"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ParentInputParameter<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string logicalName, string message, string parameterName)
        {
            var parent = ctx.ParentContext(logicalName, message);

            if (parent != null && parent.InputParameters != null && parent.InputParameters.Contains(parameterName))
            {
                return (T)parent.InputParameters[parameterName];
            }
            return default(T);
        }

        /// <summary>
        /// return parent context matching logicalname and message, null if no so parent exists in the pipeline
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="logicalName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static Microsoft.Xrm.Sdk.IPluginExecutionContext ParentContext(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string logicalName, string message)
        {
            if (ctx.ParentContext != null)
            {
                if (ctx.ParentContext.PrimaryEntityName == logicalName && ctx.ParentContext.MessageName == message)
                {
                    return ctx.ParentContext;
                }

                if (logicalName == null && ctx.ParentContext.PrimaryEntityName == null && ctx.ParentContext.MessageName == message)
                {
                    return ctx.ParentContext;
                }

                return ctx.ParentContext.ParentContext(logicalName, message);
            }
            return null;
        }

        public static T ValueOf<T>(this Microsoft.Xrm.Sdk.AttributeCollection attributes, string attrName)
        {
            attrName = attrName.ToLower();

            if (attributes != null && attributes.ContainsKey(attrName))
            {
                var obj = attributes[attrName];

                if (obj == null)
                {
                    return default(T);
                }

                return obj.ToTValueType<T>();
            }

            return default(T);
        }

        public static T ValueOf<T>(this Microsoft.Xrm.Sdk.AttributeCollection attributes, string attrName, T def)
        {
            attrName = attrName.ToLower();

            if (attributes != null && attributes.ContainsKey(attrName))
            {
                var obj = attributes[attrName];

                if (obj == null)
                {
                    return def;
                }

                return obj.ToTValueType<T>();
            }

            return def;
        }

        private static Dictionary<string, string> entityToPrimaryKey = new Dictionary<string, string>();

        public static string PrimaryAttributeNameOf(this Microsoft.Xrm.Sdk.Entity entity)
        {
            lock (entityToPrimaryKey)
            {
                if (entityToPrimaryKey.TryGetValue(entity.LogicalName, out string v))
                {
                    return v;
                }
                var strong = entity.ToEarlyBoundEntity();
                var prop = strong.GetType().GetProperty("Id");
                var attr = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).Single();

                entityToPrimaryKey.Add(entity.LogicalName, attr.LogicalName);
                return attr.LogicalName;
            }
        }

        public static bool IsOnlyPayload(this Microsoft.Xrm.Sdk.Entity entity, params string[] expectedAttributes)
        {
            return entity.Attributes.IsOnlyPayload(entity.LogicalName, expectedAttributes);
        }

        public static bool IsOnlyPayload(this Microsoft.Xrm.Sdk.AttributeCollection attrs, string entitylogicalname, params string[] expectedAttributes)
        {
            var result = false;
            expectedAttributes = expectedAttributes.Select(r => r.ToLower()).ToArray();

            var entitykey = new Microsoft.Xrm.Sdk.Entity { LogicalName = entitylogicalname }.PrimaryAttributeNameOf();
            foreach (var key in attrs.Keys)
            {
                switch (key)
                {
                    case "createdon":
                    case "createdby":
                    case "createdonbehalfby":
                    case "modifiedon":
                    case "modifiedby":
                    case "modifiedonbehalfby":
                        continue;
                    default:
                        {
                            if (key == entitykey)
                            {
                                continue;
                            }

                            if (expectedAttributes.Contains(key))
                            {
                                result = true;
                                continue;
                            }
                            return false;
                        }
                }
            }
            return result;
        }

        public static T ToTValueType<T>(this object value)
        {
            if (value is T t)
            {
                return t;
            }

            var type = typeof(T);
            var nullType = Nullable.GetUnderlyingType(type);
            if (nullType != null)
            {
                type = nullType;
            }

            if (type.IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }

            {
                if (type.IsEnum && value is Microsoft.Xrm.Sdk.OptionSetValue os)
                {
                    return (T)Enum.Parse(type, os.Value.ToString());
                }
            }

            if (value is Microsoft.Xrm.Sdk.OptionSetValueCollection oss)
            {
                if (type.IsArray)
                {
                    var arrayType = type.GetElementType();

                    if (arrayType.IsEnum)
                    {
                        var array = Array.CreateInstance(arrayType, oss.Count);
                        var ix = 0;
                        foreach (var v in oss)
                        {
                            array.SetValue(Enum.Parse(arrayType, v.Value.ToString()), ix);
                            ix++;
                        }
                        return (T)(object)array;
                    }
                }
            }
            throw new InvalidPluginExecutionException($"Unable to convert value of type {value.GetType().FullName} to {typeof(T).FullName}");
        }

        public static T ParentPreimage<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message) where T : Entity, new()
        {
            var result = new T();

            if (ctx.ParentContext == null)
            {
                return null;
            }

            if (ctx.ParentContext.MessageName == message && ctx.ParentContext.PrimaryEntityName == result.LogicalName)
            {
                if (ctx.ParentContext.PreEntityImages != null && ctx.ParentContext.PreEntityImages.Count > 0)
                {
                    foreach (Microsoft.Xrm.Sdk.Entity ent in ctx.ParentContext.PreEntityImages.Values)
                    {
                        foreach (var attr in ent.Attributes)
                        {
                            result[attr.Key] = attr.Value;
                        }
                    }
                    return result;
                }
                else
                {
                    return null;
                }
            }

            return ctx.ParentContext.ParentContext.ParentPreimage<T>(message);
        }

        public static T ParentPostimage<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message) where T : Entity, new()
        {
            if (ctx.ParentContext == null)
            {
                return null;
            }

            var result = new T();

            if (ctx.ParentContext.MessageName == message && ctx.ParentContext.PrimaryEntityName == result.LogicalName)
            {
                if (ctx.ParentContext.PostEntityImages != null && ctx.ParentContext.PostEntityImages.Count > 0)
                {
                    foreach (Microsoft.Xrm.Sdk.Entity ent in ctx.ParentContext.PostEntityImages.Values)
                    {
                        foreach (var attr in ent.Attributes)
                        {
                            result[attr.Key] = attr.Value;
                        }
                    }
                    return result;
                }
                else
                {
                    return null;
                }
            }
            return ctx.ParentContext.ParentContext.ParentPreimage<T>(message);
        }


        public static void ReplaceEmptyReferenceWithNull(this Microsoft.Xrm.Sdk.Entity target, params string[] attributes)
        {
            foreach (var value in target.Attributes.ToArray())
            {
                if (attributes != null && attributes.Length > 0 && !attributes.Contains(value.Key))
                {
                    continue;
                }

                if (value.Value is Microsoft.Xrm.Sdk.EntityReference r && r.Id == Guid.Empty)
                {
                    target.Attributes[value.Key] = null;
                }
            }
        }

        private static TargetFilterAttribute GetTargetFilterAttribute(this Type entityType, Type interfaceType)
        {
            var properties = entityType.GetCustomAttributes(Reflection.TypeCache.Types.TargetFilterAttribute, false);

            if (properties != null && properties.Length > 0)
            {
                foreach (var prop in properties)
                {
                    var type = prop.GetType().GetProperty("Type")?.GetValue(prop) as Type;
                    var attr = prop.GetType().GetProperty("Attributes")?.GetValue(prop) as string[];
                    if (type != null && attr != null && type == interfaceType)
                    {
                        return new TargetFilterAttribute(type, attr);
                    }
                }
            }
            return null;
        }

        private class TargetFilterAttribute
        {
            internal TargetFilterAttribute(Type type, string[] attributes)
            {
                this.Type = type;
                this.Attributes = attributes.Select(r => r.ToLower()).ToArray();
            }

            internal Type Type { get; private set; }
            internal string[] Attributes { get; private set; }
        }

    }
}
