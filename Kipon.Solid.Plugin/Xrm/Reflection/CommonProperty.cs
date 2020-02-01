namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommonProperty
    {

        public class Cache
        {
            private readonly Dictionary<Type, CommonProperty[]> cache = new Dictionary<Type, CommonProperty[]>();
            private readonly Types Types;

            public Cache(System.Reflection.Assembly assm)
            { 
                this.Types = Types.Instance;
            }

            public CommonProperty[] ForType(Type interfaceType, Type entityType)
            {
                if (cache.ContainsKey(interfaceType))
                {
                    return cache[interfaceType];
                }

                var interfaceProperties = interfaceType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var instanceProperties = entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                var result = new List<CommonProperty>();

                foreach (var interfaceProp in interfaceProperties)
                {
                    var instanceProp = (from i in instanceProperties where i.Name == interfaceProp.Name select i).SingleOrDefault();
                    if (instanceProp != null)
                    {
                        var customProp = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)instanceProp.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                        if (customProp != null)
                        {
                            var attr = new CommonProperty { LogicalName = customProp.LogicalName };
                            attr.Required = interfaceProp.GetCustomAttributes(Types.RequiredAttribute, false).Any();

                            result.Add(attr);
                        }
                    }
                }
                cache[interfaceType] = result.ToArray();
                return cache[interfaceType];
            }

        }

        private static readonly Dictionary<CommonProperty.Key, CommonProperty[]> cache = new Dictionary<CommonProperty.Key, CommonProperty[]>();

        private static Types Types;

        static CommonProperty()
        {
            CommonProperty.Types = Types.Instance;
        }

        public static CommonProperty[] ForType(Type interfaceType, Type entityType)
        {
            var key = new CommonProperty.Key() { FromType = interfaceType, ToType = entityType };
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var interfaceProperties = interfaceType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var instanceProperties = entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var result = new List<CommonProperty>();

            foreach (var interfaceProp in interfaceProperties)
            {
                if (interfaceProp.GetGetMethod() == null)
                {
                    continue;
                }

                if (interfaceProp.Name == nameof(Microsoft.Xrm.Sdk.Entity.Id))
                {
                    continue;
                }

                if (interfaceProp.Name == nameof(Microsoft.Xrm.Sdk.Entity.LogicalName))
                {
                    continue;
                }

                var instanceProp = (from i in instanceProperties where i.Name == interfaceProp.Name select i).SingleOrDefault();
                if (instanceProp != null)
                {
                    var customProp = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)instanceProp.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                    if (customProp != null)
                    {
                        var attr = new CommonProperty { LogicalName = customProp.LogicalName };
                        attr.Required = interfaceProp.GetCustomAttributes(Types.RequiredAttribute, false).Any();
                        attr.TargetFilter = interfaceProp.GetCustomAttributes(Types.TargetFilterAttribute, false).Any();

                        result.Add(attr);
                    }
                }
            }
            cache[key] = result.ToArray();
            return cache[key];
        }

        public string LogicalName { get; private set; }
        public bool Required { get; private set; }
        public bool TargetFilter { get; private set; }

        private class Key
        {
            internal Type FromType { get; set; }
            internal Type ToType { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Key;

                if (other != null)
                {
                    return this.FromType == other.FromType && this.ToType == other.ToType;
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + FromType.GetHashCode();
                    hash = hash * 23 + ToType.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
