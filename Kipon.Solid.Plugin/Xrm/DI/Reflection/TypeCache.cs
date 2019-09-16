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

        public static TypeCache ForType(Type type, string parameterName = null)
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

                    if (!string.IsNullOrEmpty(parameterName) && parameterName.ToLower() == "target" && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsTarget = true;
                    }
                    else
                    if (!string.IsNullOrEmpty(parameterName) && parameterName.ToLower() == "preimage" && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsPreimage = true;
                    }
                    else
                    if (!string.IsNullOrEmpty(parameterName) && parameterName.ToLower() == "mergedimage" && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsMergedimage = true;
                    }
                    else
                    if (!string.IsNullOrEmpty(parameterName) && parameterName.ToLower() == "postimage" && type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        isEntity = true;
                        resolvedTypes[type].IsPostimage = true;
                    }

                    if (!isEntity)
                    {
                        resolvedTypes[type].Constructor = GetConstructor(type);
                    }

                    return resolvedTypes[type];
                }
            }
            #endregion

            #region see if it is target, preimage post image or merged image
            if (type.IsInterface && type.IsGenericType && type.GetGenericArguments().Length == 1)
            {
                if (type.Inheriting(typeof(Target<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsTarget = true };
                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Preimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsPreimage = true };
                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Mergedimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsMergedimage = true };
                    return ReturnIfOk(type, result);
                }

                if (type.Inheriting(typeof(Mergedimage<>)))
                {
                    var result = new TypeCache { FromType = type, ToType = type.GetGenericArguments()[0], IsPostimage = true };
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

            throw new Exceptions.UnresolvableTypeException(type);
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
                throw new Exceptions.UnresolvableTypeException(type);
            }

            var all = candidates.ToArray();
            candidates.Clear();
            foreach (var t in all)
            {
                var exported = t.GetCustomAttributes(typeof(Attributes.ExportAttribute), false).Any();
                if (exported)
                {
                    candidates.Add(t);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new Exceptions.UnresolvableTypeException(type);
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
                var ca = c.GetCustomAttributes(typeof(Attributes.ImportingConstructorAttribute), false).FirstOrDefault();
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

        private static TypeCache ReturnIfOk(Type from, TypeCache result)
        {
            if (from.IsInterface && !from.IsAssignableFrom(result.ToType))
            {
                throw new Exceptions.TypeMismatchException(from, result.ToType);
            }

            result.Constructor = GetConstructor(result.ToType);

            resolvedTypes[from] = result;
            return resolvedTypes[from];
        }

        public Type FromType { get; private set; }
        public Type ToType { get; private set; }

        public System.Reflection.ConstructorInfo Constructor { get; private set; }

        public bool IsTarget { get; private set; }
        public bool IsPreimage { get; private set; }
        public bool IsMergedimage { get; private set; }
        public bool IsPostimage { get; set; }
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
    }
}
