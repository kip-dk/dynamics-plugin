namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Type cache is used to resolved types for each parameter in a context. 
    /// If the parameter is related to images (target, pre, post etc.), the type will be cached on together with the parameter info, so each parameter has its own
    /// while simple interfaces etc. will be cached on the interface type level, because no information from the parameter context is needed.
    /// </summary>
    public class TypeCache
    {
        private static Dictionary<System.Reflection.ParameterInfo, TypeCache> resolvedTypes = new Dictionary<System.Reflection.ParameterInfo, TypeCache>();

        public static TypeCache ForParameter(System.Reflection.ParameterInfo parameter)
        {
            var type = parameter.ParameterType;

            if (resolvedTypes.ContainsKey(parameter))
            {
                return resolvedTypes[parameter];
            }

            if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
            {
                resolvedTypes[parameter] = new TypeCache { FromType = type, ToType = type };

                resolvedTypes[parameter].RequireAdminService = parameter.GetCustomAttributes(typeof(Kipon.Xrm.Attributes.AdminAttribute), false).Any();
                return resolvedTypes[parameter];
            }

            if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory))
            {
                resolvedTypes[parameter] = new TypeCache { FromType = type, ToType = type };
                return resolvedTypes[parameter];
            }

            if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext))
            {
                resolvedTypes[parameter] = new TypeCache { FromType = type, ToType = type };
                return resolvedTypes[parameter];
            }

            if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.ITracingService))
            {
                resolvedTypes[parameter] = new TypeCache { FromType = type, ToType = type };
                return resolvedTypes[parameter];
            }

            #region not an abstract, and not an interface, the type can be used directly, see if the name indicates that it is target, preimage, mergedimage or postimage
            if (!type.IsInterface && !type.IsAbstract)
            {
                var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );

                if (constructors != null && constructors.Length > 0)
                {
                    var result = new TypeCache { FromType = type, ToType = type };
                    var isEntity = false;

                    #region see if we can resolve parameter to the target as an entity
                    if (parameter.MatchPattern(typeof(Type), "target") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsTarget = true;
                    }
                    else
                    if (parameter.MatchPattern(Types.PreimageAttribute, "preimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsPreimage = true;
                    }
                    else
                    if (parameter.MatchPattern(Types.MergedimageAttribute, "mergedimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsMergedimage = true;
                    }
                    else
                    if (parameter.MatchPattern(Types.PostimageAttribute, "postimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsPostimage = true;
                    }

                    if (isEntity)
                    {
                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(type);
                        result.LogicalName = entity.LogicalName;
                        result.ResolveProperties();
                    }
                    #endregion

                    var isReference = false;

                    #region see if we can resolve parameter to the target as en entity reference
                    if (!isEntity && type.ExtendsGenericClassOf(typeof(Kipon.Xrm.TargetReference<>)))
                    {
                        isReference = true;
                        result.IsTarget = true;
                        result.IsReference = true;
                        result.ToType = type.BaseType.GetGenericArguments()[0];

                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                        result.LogicalName = entity.LogicalName;
                        result.Constructor = type.GetConstructor(new Type[] { typeof(Microsoft.Xrm.Sdk.EntityReference) });
                    }

                    if (!isEntity  && !isReference && type == typeof(Microsoft.Xrm.Sdk.EntityReference))
                    {
                        if (parameter.MatchPattern(Types.TargetAttribute, "target"))
                        {
                            isReference = true;
                            result.IsTarget = true;
                            result.IsReference = true;
                            result.ToType = type;
                            result.Constructor = null;
                        }
                    }
                    #endregion

                    if (!isEntity && !isReference)
                    {
                        result.Constructor = GetConstructor(type);
                        resolvedTypes[parameter] = result;
                    } else
                    {
                        resolvedTypes[parameter] = result;
                    }
                    return result;
                }
            }
            #endregion

            #region see if it is target, preimage post image or merged image interface
            if (type.IsInterface)
            {
                Type toType = type.ImplementsGenericInterface(Types.Target);
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsTarget = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    result.ResolveProperties();

                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[parameter] = result;
                        return result;
                    }
                }

                toType = type.ImplementsGenericInterface(Types.Preimage);
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsPreimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    result.ResolveProperties();
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[parameter] = result;
                        return resolvedTypes[parameter];
                    }
                }

                toType = type.ImplementsGenericInterface(Types.Mergedimage);
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsMergedimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    result.ResolveProperties();
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[parameter] = result;
                        return resolvedTypes[parameter];
                    }
                }

                toType = type.ImplementsGenericInterface(Types.Postimage);
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsPostimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    result.ResolveProperties();
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[parameter] = result;
                        return result;
                    }
                }
            }
            #endregion

            #region find implementing interface
            if (type.IsInterface)
            {
                var r1 = GetInterfaceImplementation(type);
                var result = new TypeCache { FromType = type, ToType = r1, Constructor = GetConstructor(r1) };

                resolvedTypes[parameter] = result;
                return result;

            }
            #endregion

            #region find relevant abstract extension
            if (type.IsAbstract)
            {
            }
            #endregion

            throw new Kipon.Xrm.Exceptions.UnresolvableTypeException(type);
        }

        #region private static helpers
        private static Type GetInterfaceImplementation(Type type)
        {
            var allTypes = Types.Assembly.GetTypes();
            var candidates = new List<Type>();

            foreach (var t in allTypes)
            {
                if (!t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t))
                {
                    var cons = t.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (cons.Length > 0)
                    {
                        candidates.Add(t);
                    }
                }
            }

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            if (candidates.Count == 0)
            {
                throw new Kipon.Xrm.Exceptions.UnresolvableTypeException(type);
            }

            var all = candidates.ToArray();
            candidates.Clear();
            foreach (var t in all)
            {
                var exported = (Kipon.Xrm.Attributes.ExportAttribute)t.GetCustomAttributes(typeof(Kipon.Xrm.Attributes.ExportAttribute), false).SingleOrDefault();
                if (exported != null && exported.Type == type)
                {
                    candidates.Add(t);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new Kipon.Xrm.Exceptions.MultiImplementationOfSameInterfaceException(type);
        }

        private static System.Reflection.ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            if (constructors == null || constructors.Length == 0)
            {
                return null;
            }

            List<System.Reflection.ConstructorInfo> candidates = new List<System.Reflection.ConstructorInfo>();
            foreach (var c in constructors)
            {
                var ca = c.GetCustomAttributes(typeof(Kipon.Xrm.Attributes.ImportingConstructorAttribute), false).FirstOrDefault();
                if (ca != null)
                {
                    candidates.Add(c);
                }
            }
            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new Kipon.Xrm.Exceptions.UnresolvableConstructorException(type);
        }

        private static bool ReturnIfOk(Type from, TypeCache result)
        {
            if (from.IsInterface && !from.IsAssignableFrom(result.ToType))
            {
                throw new Kipon.Xrm.Exceptions.TypeMismatchException(from, result.ToType);
            }
            return true;
        }
        #endregion

        #region private helpers
        private void ResolveProperties()
        {
            if (this.FromType.Inheriting(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                this.AllProperties = true;
                return;
            }

            if (this.ToType.Inheriting(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                this.FilteredProperties = CommonPropertyCache.ForType(this.FromType, this.ToType);
            }
        }
        #endregion

        #region properties
        public Type FromType { get; private set; }
        public Type ToType { get; private set; }

        public System.Reflection.ConstructorInfo Constructor { get; private set; }

        public bool IsTarget { get; private set; }
        public bool IsReference { get; private set; }
        public bool IsPreimage { get; private set; }
        public bool IsMergedimage { get; private set; }
        public bool IsPostimage { get; private set; }
        public string LogicalName { get; private set; }

        public bool RequireAdminService { get; private set; }
        public bool AllProperties { get; private set; }
        public CommonPropertyCache[] FilteredProperties { get; private set; }

        public bool RequirePluginContext
        {
            get
            {
                return IsTarget || IsReference || IsPreimage || IsPostimage || IsMergedimage;
            }
        }

        private string _ik;
        public string ObjectInstanceKey
        {
            get
            {
                if (_ik == null)
                {
                    if (this.IsTarget) this._ik = "target:" + this.ToType.FullName;
                    else if (this.IsPreimage) this._ik = "preimage:" + this.ToType.FullName;
                    else if (this.IsMergedimage) this._ik = "mergedimage:" + this.ToType.FullName;
                    else if (this.IsPostimage) this._ik = "postimage:" + this.ToType.FullName;
                    else if (this.IsReference) this._ik = "ref:" + this.ToType.FullName;
                    else if (this.FromType.Implements(typeof(Kipon.Xrm.IAdminUnitOfWork)))
                    {
                        this._ik = typeof(Kipon.Xrm.IAdminUnitOfWork).FullName;
                        this.RequireAdminService = true;
                    }
                    else if (this.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                    {
                        if (this.RequireAdminService)
                        {
                            this._ik = this.FromType.FullName + ":admin";
                        } else
                        {
                            this._ik = this.FromType.FullName;
                        }
                    }
                    else if (this.FromType.Implements(typeof(Kipon.Xrm.IUnitOfWork))) this._ik = typeof(Kipon.Xrm.IUnitOfWork).FullName;
                    else if (this.ToType != null) this._ik = this.ToType.FullName;
                    else this._ik = this.FromType.FullName;
                }
                return _ik;
            }
        }
        #endregion
    }


    public static class TypeCacheLocalExtensions
    {
        public static bool Inheriting(this Type value, Type other)
        {
            if (value.BaseType == other)
            {
                return true;
            }

            if (value.IsSubclassOf(other))
            {
                return true;
            }

            if (value.BaseType != null)
            {
                return value.BaseType.Inheriting(other);
            }
            return false;
        }

        public static bool Implements(this Type value, Type other)
        {
            var intf = value.GetInterfaces();
            return intf != null && intf.Contains(other);
        }

        public static Type ImplementsGenericInterface(this Type value, Type other)
        {
            if (!other.IsGenericType)
            {
                return null;
            }

            var match = value.GetInterfaces()
                .Where(i => i.IsGenericType)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == other);

            if (match != null)
            {
                return match.GetGenericArguments()[0];
            }

            return null;
        }


        public static bool ExtendsGenericClassOf(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool MatchPattern(this System.Reflection.ParameterInfo parameter, Type customAttribute, string attributeName)
        {
            if (parameter == null)
            {
                return false;
            }
            var hasCA = parameter.GetCustomAttributes(customAttribute, false).Any();
            if (hasCA)
            {
                return true;
            }

            return !string.IsNullOrEmpty(attributeName) && parameter.Name.ToLower().Equals(attributeName.ToLower());
        }
    }
}
