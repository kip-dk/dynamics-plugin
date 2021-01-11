namespace Kipon.Xrm.Extensions.Sdk
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk;

    public static partial class KiponSdkGeneratedExtensionMethods
    {
        private static readonly Dictionary<string, Type> entittypes = new Dictionary<string, Type>();
        private static readonly Dictionary<string, System.Reflection.MethodInfo> TO_ENT_GENS = new Dictionary<string, System.Reflection.MethodInfo>();
        private static readonly System.Reflection.MethodInfo TO_ENTITY = typeof(Microsoft.Xrm.Sdk.Entity).GetMethod("ToEntity", new Type[0]);
        private static readonly Dictionary<string, string[]> targetattributes = new Dictionary<string, string[]>();

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
        /// <returns></returns>
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

            if (message == "Create" || message == "Update" || message == "Delete")
            {
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
                            }
                        }
                    }
                }
            }
            return ctx.ParentContext.IsChildOf(message, entityLogicalName);
        }

        /// <summary>
        /// Returns true if the attribute is part of the target payload of the request. It only make sense in Create, Update requests
        /// </summary>
        /// <param name="ctx">The plugin execution context</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>True if context contains Target as an entity and the entity has the attribute set.</returns>
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
