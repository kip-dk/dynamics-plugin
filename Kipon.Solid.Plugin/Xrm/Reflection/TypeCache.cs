namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Type cache is used to resolved types for each parameter in a context. 
    /// If the parameter is related to images (target, pre, post etc.), 
    /// The type is cached on the pointer to the parameter, so each parameter is only resolved 
    /// once in the system life-time.
    /// </summary>
    public class TypeCache
    {
        public static Types Types { get; set; }

        private static System.Reflection.ParameterInfo UOW;
        private static System.Reflection.ParameterInfo UOW_ADMIN;

        private static readonly object locks = new object();

        static TypeCache()
        {
            TypeCache.Types = Types.Instance;
            var method = typeof(TypeCache).GetMethod(nameof(TypeCache.DummyUOW), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pms = method.GetParameters();
            UOW = pms[0];
            UOW_ADMIN = pms[1];
        }

        private static Dictionary<Key, TypeCache> resolvedTypes = new Dictionary<Key, TypeCache>();

        public static TypeCache ForParameter(System.Reflection.ParameterInfo parameter, string logicalname)
        {
            var key = new Key() { Parameter = parameter, LogicalName = logicalname };

            var type = parameter.ParameterType;

            if (resolvedTypes.ContainsKey(key))
            {
                return resolvedTypes[key];
            }

            lock (locks)
            {
                if (resolvedTypes.ContainsKey(key))
                {
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Guid))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(string))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(int))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(int?))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(decimal))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(decimal?))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(bool))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(bool?))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.EntityReference))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.OptionSetValue))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.Money))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type, Name = parameter.Name, IsInputParameter = true };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };

                    resolvedTypes[key].RequireAdminService = parameter.GetCustomAttributes(Types.AdminAttribute, false).Any();
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.ITracingService))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == Types.IPluginContext)
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.Query.QueryExpression))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, Name = parameter.Name, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.Relationship))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, Name = parameter.Name, ToType = type };
                    return resolvedTypes[key];
                }

                if (parameter.Name.ToLower() == "datasource" && (parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.Entity) || parameter.ParameterType.BaseType == typeof(Microsoft.Xrm.Sdk.Entity)))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, Name = parameter.Name, ToType = type };
                    return resolvedTypes[key];
                }

                #region not an abstract, and not an interface, the type can be used directly, see if the name indicates that it is target, preimage, mergedimage or postimage
                if (!type.IsInterface && !type.IsAbstract)
                {
                    var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                    if (constructors != null && constructors.Length > 0)
                    {
                        var result = new TypeCache { FromType = type, ToType = type };
                        var isEntity = false;

                        #region see if we can resolve parameter to the target as an entity
                        if (parameter.MatchPattern(Types.TargetAttribute, "target") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
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
                        if (!isEntity && type.ExtendsGenericClassOf(Types.TargetReference))
                        {
                            isReference = true;
                            result.IsTarget = true;
                            result.IsReference = true;
                            result.ToType = type.BaseType.GetGenericArguments()[0];

                            var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                            result.LogicalName = entity.LogicalName;
                            result.Constructor = type.GetConstructor(new Type[] { typeof(Microsoft.Xrm.Sdk.EntityReference) });
                        }

                        if (!isEntity && !isReference && type == typeof(Microsoft.Xrm.Sdk.EntityReference))
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
                            resolvedTypes[key] = result;
                        }
                        else
                        {
                            resolvedTypes[key] = result;
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
                            resolvedTypes[key] = result;
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
                            resolvedTypes[key] = result;
                            return resolvedTypes[key];
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
                            resolvedTypes[key] = result;
                            return resolvedTypes[key];
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
                            resolvedTypes[key] = result;
                            return result;
                        }
                    }

                    if (type.IsITarget())
                    {
                        var entity = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(new Microsoft.Xrm.Sdk.Entity { LogicalName = key.LogicalName });
                        var entityType = entity.GetType();

                        if (type.IsAssignableFrom(entityType))
                        {
                            var result = new TypeCache { FromType = type, ToType = entityType, IsTarget = true };
                            result.LogicalName = entity.LogicalName;
                            result.ResolveProperties();

                            if (ReturnIfOk(type, result))
                            {
                                resolvedTypes[key] = result;
                                return result;
                            }
                        }
                    }
                }
                #endregion

                #region handle shared interface by naming convention or attribute decoration
                if (type.IsInterface && !string.IsNullOrEmpty(key.LogicalName))
                {
                    var isTarget = parameter.Name == "target";

                    if (!isTarget)
                    {
                        isTarget = parameter.GetCustomAttributes(Types.TargetAttribute, false).Any();
                    }

                    if (isTarget)
                    {
                        var entity = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(new Microsoft.Xrm.Sdk.Entity { LogicalName = key.LogicalName });

                        var result = new TypeCache { FromType = type, ToType = entity.GetType(), IsTarget = true };
                        result.LogicalName = key.LogicalName;
                        result.IsGenericEntityInterface = true;
                        result.ResolveProperties();
                        result.IsImplemenedByEntity = ReturnIfImplemented(type, result);
                        resolvedTypes[key] = result;
                        return result;
                    }
                }

                if (type.IsInterface && !string.IsNullOrEmpty(key.LogicalName))
                {
                    var isPreimage = parameter.Name == "preimage";

                    if (!isPreimage)
                    {
                        isPreimage = parameter.GetCustomAttributes(Types.PreimageAttribute, false).Any();
                    }

                    if (isPreimage)
                    {
                        var entity = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(new Microsoft.Xrm.Sdk.Entity { LogicalName = key.LogicalName });

                        var result = new TypeCache { FromType = type, ToType = entity.GetType(), IsPreimage = true };
                        result.LogicalName = key.LogicalName;
                        result.IsGenericEntityInterface = true;
                        result.ResolveProperties();
                        result.IsImplemenedByEntity = ReturnIfImplemented(type, result);
                        resolvedTypes[key] = result;
                        return result;
                    }
                }

                if (type.IsInterface && !string.IsNullOrEmpty(key.LogicalName))
                {
                    var isMergedimage = parameter.Name == "mergedimage";

                    if (!isMergedimage)
                    {
                        isMergedimage = parameter.GetCustomAttributes(Types.MergedimageAttribute, false).Any();
                    }

                    if (isMergedimage)
                    {
                        var entity = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(new Microsoft.Xrm.Sdk.Entity { LogicalName = key.LogicalName });

                        var result = new TypeCache { FromType = type, ToType = entity.GetType(), IsMergedimage = true };
                        result.LogicalName = key.LogicalName;
                        result.IsGenericEntityInterface = true;
                        result.ResolveProperties();
                        result.IsImplemenedByEntity = ReturnIfImplemented(type, result);
                        resolvedTypes[key] = result;
                        return result;
                    }
                }

                if (type.IsInterface && !string.IsNullOrEmpty(key.LogicalName))
                {
                    var isPostimage = parameter.Name == "postimage";

                    if (!isPostimage)
                    {
                        isPostimage = parameter.GetCustomAttributes(Types.PostimageAttribute, false).Any();
                    }

                    if (isPostimage)
                    {
                        var entity = Extensions.Sdk.KiponSdkGeneratedExtensionMethods.ToEarlyBoundEntity(new Microsoft.Xrm.Sdk.Entity { LogicalName = key.LogicalName });

                        var result = new TypeCache { FromType = type, ToType = entity.GetType(), IsPostimage = true };
                        result.LogicalName = key.LogicalName;
                        result.IsGenericEntityInterface = true;
                        result.ResolveProperties();
                        result.IsImplemenedByEntity = ReturnIfImplemented(type, result);
                    }
                }
                #endregion

                #region IQueryable
                if (type.IsInterface && type.IsGenericType && type.GenericTypeArguments.Length == 1 && type.GenericTypeArguments[0].BaseType != null && type.GenericTypeArguments[0].BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    var result = ForQuery(type);
                    if (result != null)
                    {
                        result.RequireAdminService = parameter.GetCustomAttributes(Types.AdminAttribute, false).Any();
                        return result;
                    }
                }
                #endregion

                #region IRepository
                if (type.IsInterface && type.IsGenericType && type.GenericTypeArguments.Length == 1 && type.GenericTypeArguments[0].BaseType != null && type.GenericTypeArguments[0].BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    var result = ForRepository(type);
                    if (result != null)
                    {
                        result.RequireAdminService = parameter.GetCustomAttributes(Types.AdminAttribute, false).Any();
                        return result;
                    }
                }
                #endregion

                if (type == Types.IEntityCache)
                {
                    var result = ForEntityCache(type);
                    result.RequireAdminService = parameter.GetCustomAttributes(Types.AdminAttribute, false).Any();
                    return result;
                }

                #region find implementing interface
                if (type.IsInterface)
                {
                    var r1 = GetInterfaceImplementation(type);

                    var entityType = type.ImplementsGenericInterface(Types.ActionTarget);
                    string logialName = null;
                    bool isActionReference = false;

                    if (entityType != null)
                    {
                        logialName = ((Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(entityType)).LogicalName;
                        isActionReference = true;
                    }

                    var result = new TypeCache { FromType = type, ToType = r1, Constructor = GetConstructor(r1), LogicalName = logialName, IsActionReference = isActionReference };
                    result.RequireAdminService = parameter.GetCustomAttributes(Types.AdminAttribute, false).Any();

                    resolvedTypes[key] = result;
                    return result;
                }
                #endregion

                throw new Exceptions.UnresolvableTypeException(type);
            }
        }

        public static TypeCache ForProperty(System.Reflection.PropertyInfo property)
        {
            var key = new Key() { Property = property };

            var type = property.PropertyType;

            if (resolvedTypes.ContainsKey(key))
            {
                return resolvedTypes[key];
            }

            lock (locks)
            {
                if (resolvedTypes.ContainsKey(key))
                {
                    return resolvedTypes[key];
                }

                if (property.PropertyType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };

                    resolvedTypes[key].RequireAdminService = property.GetCustomAttributes(Types.AdminAttribute, false).Any();
                    return resolvedTypes[key];
                }

                if (property.PropertyType == typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (property.PropertyType == typeof(Microsoft.Xrm.Sdk.Workflow.IWorkflowContext))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                if (property.PropertyType == typeof(Microsoft.Xrm.Sdk.ITracingService))
                {
                    resolvedTypes[key] = new TypeCache { FromType = type, ToType = type };
                    return resolvedTypes[key];
                }

                #region IQueryable
                if (type.IsInterface && type.IsGenericType && type.GenericTypeArguments.Length == 1 && type.GenericTypeArguments[0].BaseType != null && type.GenericTypeArguments[0].BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    var result = ForQuery(type);
                    if (result != null)
                    {
                        result.RequireAdminService = property.GetCustomAttributes(Types.AdminAttribute, false).Any();
                        return result;
                    }
                }
                #endregion

                #region IRepository
                if (type.IsInterface && type.IsGenericType && type.GenericTypeArguments.Length == 1 && type.GenericTypeArguments[0].BaseType != null && type.GenericTypeArguments[0].BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    var result = ForRepository(type);
                    if (result != null)
                    {
                        result.RequireAdminService = property.GetCustomAttributes(Types.AdminAttribute, false).Any();
                        return result;
                    }
                }
                #endregion

                #region find implementing interface
                if (type.IsInterface)
                {
                    var r1 = GetInterfaceImplementation(type);

                    var entityType = type.ImplementsGenericInterface(Types.ActionTarget);
                    string logialName = null;
                    bool isActionReference = false;

                    if (entityType != null)
                    {
                        logialName = ((Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(entityType)).LogicalName;
                        isActionReference = true;
                    }

                    var result = new TypeCache { FromType = type, ToType = r1, Constructor = GetConstructor(r1), LogicalName = logialName, IsActionReference = isActionReference };
                    result.RequireAdminService = property.GetCustomAttributes(Types.AdminAttribute, false).Any();

                    resolvedTypes[key] = result;
                    return result;
                }
                #endregion

                throw new Exceptions.UnresolvableTypeException(type);
            }
        }

        public static TypeCache ForQuery(Type type)
        {
            var genericQueryable = typeof(System.Linq.IQueryable<>);
            var queryType = genericQueryable.MakeGenericType(type.GenericTypeArguments[0]);
            if (type == queryType)
            {
                var result = new TypeCache { FromType = type, ToType = queryType, IsQuery = true };
                return result;
            }
            return null;
        }

        public static TypeCache ForRepository(Type type)
        {
            var repositoryType = Types.IRepository;
            var queryType = repositoryType.MakeGenericType(type.GenericTypeArguments[0]);
            if (type == queryType)
            {
                var result = new TypeCache { FromType = type, ToType = queryType, IsRepository = true };
                return result;
            }
            return null;
        }

        public static TypeCache ForEntityCache(Type type)
        {
            return new TypeCache { FromType = type, IsEntityCache = true };
        }


        public static TypeCache ForUow(bool admin)
        {
            var pi = admin ? UOW_ADMIN : UOW;

            var key = new Key { Parameter = pi };
            if (resolvedTypes.ContainsKey(key))
            {
                return resolvedTypes[key];
            }

            var fromType = admin ? Types.IAdminUnitOfWork : Types.IUnitOfWork;
            var r1 = GetInterfaceImplementation(fromType);
            var result = new TypeCache { FromType = fromType, ToType = r1, Constructor = GetConstructor(r1), RequireAdminService = admin };
            resolvedTypes[key] = result;
            return result;
        }

        private void DummyUOW(object uow, object adminUOW)
        {
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
                throw new Exceptions.UnresolvableTypeException(type);
            }

            var all = candidates.ToArray();
            candidates.Clear();
            foreach (var t in all)
            {
                var exports = t.GetCustomAttributes(Types.ExportAttribute, false).ToArray();
                foreach (var exported in exports)
                {
                    if (exported != null && (Type)exported.GetType().GetProperty("Type").GetValue(exported) == type)
                    {
                        candidates.Add(t);
                    }
                }
            }

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new Exceptions.MultiImplementationOfSameInterfaceException(type);
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
                var ca = c.GetCustomAttributes(Types.ImportingConstructorAttribute, false).FirstOrDefault();
                if (ca != null)
                {
                    candidates.Add(c);
                }
            }
            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new Exceptions.UnresolvableConstructorException(type);
        }

        private static bool ReturnIfOk(Type from, TypeCache result)
        {
            if (from.IsInterface && !from.IsAssignableFrom(result.ToType))
            {
                throw new Exceptions.TypeMismatchException(from, result.ToType);
            }
            return true;
        }


        private static bool ReturnIfImplemented(Type from, TypeCache result)
        {
            if (from.IsInterface && !from.IsAssignableFrom(result.ToType))
            {
                return false;
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
                this.FilteredProperties = CommonProperty.ForType(this.FromType, this.ToType);
            }
        }
        #endregion

        #region properties
        public Type FromType { get; private set; }
        public Type ToType { get; private set; }
        public string Name { get; private set; }
        public System.Reflection.ConstructorInfo Constructor { get; private set; }
        public bool IsGenericEntityInterface { get; private set; }
        public bool IsImplemenedByEntity { get; private set; }
        public bool IsTarget { get; private set; }
        public bool IsReference { get; private set; }
        public bool IsActionReference { get; private set; }
        public bool IsPreimage { get; private set; }
        public bool IsMergedimage { get; private set; }
        public bool IsPostimage { get; private set; }
        public string LogicalName { get; private set; }
        public bool IsQuery { get; private set; }
        public bool IsRepository { get; private set; }
        public bool IsEntityCache { get; private set; }
        public bool RequireAdminService { get; private set; }
        public bool AllProperties { get; private set; }

        public bool IsAdminUnitOfWork
        {
            get
            {
                return this.FromType.Implements(Types.IAdminUnitOfWork) ||
                    (this.FromType.Implements(Types.IUnitOfWork) && this.RequireAdminService);
            }
        }

        public bool IsUnitOfWork
        {
            get
            {
                return this.FromType.Implements(Types.IUnitOfWork) && !this.RequireAdminService;
            }
        }

        public CommonProperty[] FilteredProperties { get; private set; }

        private CommonProperty[] _targetFilterProperties;
        public CommonProperty[] TargetFilterProperties
        {
            get
            {
                if (_targetFilterProperties == null)
                {
                    if (FilteredProperties == null)
                    {
                        _targetFilterProperties = new CommonProperty[0];
                        return _targetFilterProperties;
                    }
                    _targetFilterProperties = (from f in this.FilteredProperties
                                               where f.TargetFilter
                                               select f).ToArray();
                }
                return _targetFilterProperties;
            }
        }

        public bool RequirePluginContext
        {
            get
            {
                return IsTarget || IsReference || IsPreimage || IsPostimage || IsMergedimage;
            }
        }

        public bool IsInputParameter { get; set; }

        private System.Reflection.PropertyInfo _repositoryProperty;
        public System.Reflection.PropertyInfo RepositoryProperty
        {
            get
            {
                if (_repositoryProperty != null)
                {
                    return _repositoryProperty;
                }

                var repositoryType = Types.Instance.IRepository.GetGenericTypeDefinition();
                var entityType = this.ToType.GetGenericArguments()[0];
                var queryType = repositoryType.MakeGenericType(entityType);

                var uowTC = TypeCache.ForUow(this.RequireAdminService);

                var properties = uowTC.ToType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                        .Where(r => r.PropertyType == queryType).ToArray();

                if (properties.Length == 0)
                {
                    throw new Exceptions.UnresolvableTypeException(this.ToType);
                }

                if (properties.Length > 1)
                {
                    throw new Exceptions.MultiImplementationOfSameInterfaceException(this.ToType);
                }

                this._repositoryProperty = properties[0];
                return _repositoryProperty;
            }
        }

        private System.Reflection.MethodInfo _queryMethod;
        public System.Reflection.MethodInfo QueryMethod
        {
            get
            {
                if (this._queryMethod == null)
                {
                    var repository = this.RepositoryProperty;
                    this._queryMethod = repository.PropertyType.GetMethod("GetQuery", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (this.QueryMethod == null)
                    {
                        throw new Exceptions.UnresolvableTypeException(this.ToType);
                    }
                }
                return this._queryMethod;
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
                    else if (this.FromType.Implements(Types.IAdminUnitOfWork))
                    {
                        this._ik = Types.IAdminUnitOfWork.FullName;
                        this.RequireAdminService = true;
                    }
                    else if (this.FromType.Implements(Types.IUnitOfWork) && this.RequireAdminService)
                    {
                        this._ik = Types.IAdminUnitOfWork.FullName;
                        this.RequireAdminService = true;
                    }
                    else if (this.FromType.Implements(Types.IUnitOfWork)) this._ik = Types.IUnitOfWork.FullName;
                    else if (this.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                    {
                        if (this.RequireAdminService)
                        {
                            this._ik = this.FromType.FullName + ":admin";
                        }
                        else
                        {
                            this._ik = this.FromType.FullName;
                        }
                    }
                    else if (this.IsQuery)
                    {
                        if (this.RequireAdminService)
                        {
                            return this.ToType.FullName + ":admin";
                        }
                        return this.ToType.FullName;
                    }
                    else if (this.FromType == typeof(Guid)) return $"GUID:{this.Name}";
                    else if (this.ToType != null) this._ik = this.ToType.FullName;
                    else this._ik = this.FromType.FullName;
                }
                return _ik;
            }
        }
        #endregion

        #region cache key
        private class Key
        {
            internal System.Reflection.ParameterInfo Parameter { get; set; }
            internal System.Reflection.PropertyInfo Property { get; set; }
            internal string LogicalName { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Key;
                if (other != null && Parameter != null)
                {
                    return other.LogicalName == this.LogicalName && other.Parameter == this.Parameter;
                }

                if (other != null && Property != null)
                {
                    return other.Property == this.Property;
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + Parameter.GetHashCode();
                    if (LogicalName != null)
                    {
                        hash = hash * 23 + this.LogicalName.GetHashCode();
                    }
                    return hash;
                }
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
            if (other == null)
            {
                return null;
            }

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

        public static bool IsITarget(this Type fromType)
        {
            var propertytype = TypeCache.Types.ITarget;
            return propertytype.IsAssignableFrom(fromType);
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
