using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI.Reflection
{
    /// <summary>
    /// Type cache is used to resolved types for each parameter in a context. 
    /// If the parameter is related to images (target, pre, post etc.), the type will be cached on together with the parameter info, so each parameter has its own
    /// while simple interfaces etc. will be cached on the interface type level, because no information from the parameter context is needed.
    /// </summary>
    public class TypeCache
    {
        private static Dictionary<object, TypeCache> resolvedTypes = new Dictionary<object, TypeCache>();

        public static TypeCache ForParameter(System.Reflection.ParameterInfo parameter)
        {
            var type = parameter.ParameterType;

            if (resolvedTypes.ContainsKey(type))
            {
                return resolvedTypes[type];
            }

            CombinedKey ck = new CombinedKey { Type = type, ParameterInfo = parameter };
            if (resolvedTypes.ContainsKey(ck))
            {
                return resolvedTypes[ck];
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
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.TargetAttribute), "target") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsTarget = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.PreimageAttribute), "preimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsPreimage = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.MergedimageAttribute), "mergedimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsMergedimage = true;
                    }
                    else
                    if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.PostimageAttribute), "postimage") && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        result.IsPostimage = true;
                    }

                    if (isEntity)
                    {
                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(type);
                        result.LogicalName = entity.LogicalName;
                    }
                    #endregion

                    var isReference = false;

                    #region see if we can resolve parameter to the target as en entity reference
                    if (!isEntity && type.Inheriting(typeof(Kipon.Xrm.TargetReference<>)))
                    {
                        isReference = true;
                        result.IsTarget = true;
                        result.IsReference = true;
                        result.ToType = type.GetGenericArguments()[0];

                        var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(resolvedTypes[type].ToType);
                        result.LogicalName = entity.LogicalName;
                    }

                    if (!isEntity  && !isReference && type == typeof(Microsoft.Xrm.Sdk.EntityReference))
                    {
                        if (parameter.MatchPattern(typeof(Kipon.Xrm.Attributes.TargetAttribute), "target"))
                        {
                            isReference = true;
                            result.IsTarget = true;
                            result.IsReference = true;
                            result.ToType = type;
                        }
                    }
                    #endregion

                    if (!isEntity && !isReference)
                    {
                        result.Constructor = GetConstructor(type);
                        resolvedTypes[type] = result;
                    } else
                    {
                        resolvedTypes[ck] = result;
                    }
                    return result;
                }
            }
            #endregion

            #region see if it is target, preimage post image or merged image interface
            if (type.IsInterface)
            {
                Type toType = type.ImplementsGenericInterface(typeof(Kipon.Xrm.Target<>));
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsTarget = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;

                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[ck] = result;
                        return result;
                    }
                }

                toType = type.ImplementsGenericInterface(typeof(Kipon.Xrm.Preimage<>));
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsPreimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[ck] = result;
                        return resolvedTypes[ck];
                    }
                }

                toType = type.ImplementsGenericInterface(typeof(Kipon.Xrm.Mergedimage<>));
                if (type.Implements(typeof(Kipon.Xrm.Mergedimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = toType, IsMergedimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[ck] = result;
                        return resolvedTypes[ck];
                    }
                }

                toType = type.ImplementsGenericInterface(typeof(Kipon.Xrm.Postimage<>));
                if (toType != null)
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsPostimage = true };
                    var entity = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(result.ToType);
                    result.LogicalName = entity.LogicalName;
                    if (ReturnIfOk(type, result))
                    {
                        resolvedTypes[ck] = result;
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

                resolvedTypes[result.FromType] = result;
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

        private static Type GetInterfaceImplementation(Type type)
        {
            var allTypes = typeof(TypeCache).Assembly.GetTypes();
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

        public Type FromType { get; private set; }
        public Type ToType { get; private set; }

        public System.Reflection.ConstructorInfo Constructor { get; private set; }

        public bool IsTarget { get; private set; }
        public bool IsReference { get; private set; }
        public bool IsPreimage { get; private set; }
        public bool IsMergedimage { get; private set; }
        public bool IsPostimage { get; private set; }
        public string LogicalName { get; private set; }
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

    internal class CombinedKey
    {
        internal Type Type { get; set; }
        internal System.Reflection.ParameterInfo ParameterInfo { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CombinedKey;
            if (other != null)
            {
                return this.Type == other.Type && this.ParameterInfo == other.ParameterInfo;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() + this.ParameterInfo.GetHashCode();
        }
    }
}
