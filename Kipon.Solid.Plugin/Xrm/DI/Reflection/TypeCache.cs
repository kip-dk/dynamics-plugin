using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Xrm.DI.Reflection
{
    public class TypeCache
    {
        private static Dictionary<Type, TypeCache> resolvedTypes = new Dictionary<Type, TypeCache>();

        private TypeCache()
        {
        }

        public static TypeCache ForType(Type type, System.Reflection.ParameterInfo parameter = null)
        {
            if (resolvedTypes.ContainsKey(type))
            {
                return resolvedTypes[type];
            }

            #region not an abstract, and not an interface, the type can be used directly, see if the name indicates that it is target, preimage, mergedimage or postimage
            if (!type.IsInterface && !type.IsAbstract)
            {
                var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public);

                if (constructors != null && constructors.Length > 0)
                {
                    resolvedTypes[type] = new TypeCache { FromType = type, ToType = type };
                    var isEntity = false;

                    #region see if we can resolve parameter to the target as an entity
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.TargetAttribute), "target") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsTarget = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.PreimageAttribute), "preimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsPreimage = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.MergedimageAttribute), "mergedimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsMergedimage = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.PostimageAttribute), "postimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsPostimage = true;
                    }

                    if (isEntity)
                    {
                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(type);
                        resolvedTypes[type].LogicalName = entity.LogicalName;
                    }
                    #endregion

                    var isReference = false;

                    #region see if we can resolve parameter to the target as en entity reference
                    if (!isEntity && type.Inheriting(typeof(Kipon.Xrm.TargetReference<>)))
                    {
                        isReference = true;
                        resolvedTypes[type].IsTarget = true;
                        resolvedTypes[type].IsReference = true;
                        resolvedTypes[type].ToType = type.GetGenericArguments()[0];

                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(resolvedTypes[type].ToType);
                        resolvedTypes[type].LogicalName = entity.LogicalName;
                    }

                    if (!isEntity  && !isReference && type == typeof(Microsoft.Xrm.Sdk.EntityReference))
                    {
                        if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.TargetAttribute), "target"))
                        {
                            isReference = true;
                            resolvedTypes[type].IsTarget = true;
                            resolvedTypes[type].IsReference = true;
                            resolvedTypes[type].ToType = type;
                        }
                    }
                    #endregion

                    if (!isEntity && !isReference)
                    {
                        resolvedTypes[type].Constructor = GetConstructor(type);
                    }
                    return resolvedTypes[type];
                }
            }
            #endregion

            #region see if it is target, preimage post image or merged image interface
            if (type.IsInterface && type.IsGenericType && type.GetGenericArguments().Length == 1)
            {
                if (type.Inheriting(typeof(Kipon.Xrm.Target<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsTarget = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;

                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Kipon.Xrm.Preimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsPreimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Kipon.Xrm.Mergedimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsMergedimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Kipon.Xrm.Mergedimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsPostimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    return ReturnIfOk(type, result);
                }
            }
            #endregion

            #region find implementing interface
            if (type.IsInterface)
            {
                var r1 = GetInterfaceImplementation(type);
                var result = new TypeCache { FromType = type, ToType = r1, Constructor = GetConstructor(r1) };
                return ReturnIfOk(type, result);
            }
            #endregion

            #region find relevant abstract extension
            if (type.IsAbstract)
            {
            }
            #endregion

            throw new Kipon.Xrm.Exceptions.UnresolvableTypeException(type);
        }

        private static Type GetInterfaceImplementation(Type type)
        {
            var allTypes = typeof(TypeCache).Assembly.GetTypes();
            var candidates = new List<Type>();

            foreach (var t in allTypes)
            {
                if (!t.IsInterface && !t.IsAbstract && t.IsAssignableFrom(type))
                {
                    var cons = t.GetConstructors(System.Reflection.BindingFlags.Public);
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

            throw new Kipon.Xrm.Exceptions.UnresolvableTypeException(type);
        }

        private static System.Reflection.ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public);
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

        private static TypeCache ReturnIfOk(Type from, TypeCache result)
        {
            if (from.IsInterface && !from.IsAssignableFrom(result.ToType))
            {
                throw new Kipon.Xrm.Exceptions.TypeMismatchException(from, result.ToType);
            }

            result.Constructor = GetConstructor(result.ToType);

            resolvedTypes[from] = result;
            return resolvedTypes[from];
        }

        public Type FromType { get; private set; }
        public Type ToType { get; private set; }

        public System.Reflection.ConstructorInfo Constructor { get; private set; }

        public bool IsTarget { get; private set; }
        public bool IsReference { get; private set; }
        public bool IsPreimage { get; private set; }
        public bool IsMergedimage { get; private set; }
        public bool IsPostimage { get; set; }
        public string LogicalName { get; set; }
    }


    public static class TypeCacheLocalExtensions
    {
        public static bool Inheriting(this Type value, Type other)
        {
            if (value.BaseType == other)
            {
                return true;
            }

            if (value.BaseType != null)
            {
                return value.BaseType.Inheriting(other);
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
