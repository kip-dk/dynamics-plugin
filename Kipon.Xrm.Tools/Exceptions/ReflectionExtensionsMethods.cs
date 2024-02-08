using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Exceptions
{
    public static class ReflectionExtensionsMethods
    {
        public static System.Reflection.PropertyInfo[] PublicInstanceWithDecorator(this Type type, Type decorator)
        {
            var result = new List<System.Reflection.PropertyInfo>();

            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public & System.Reflection.BindingFlags.Instance))
            {
                var cust = prop.GetCustomAttributes(type, false).FirstOrDefault();
                if (cust != null)
                {
                    result.Add(prop);
                }
            }
            return result.ToArray();
        }

        public static bool PropertyHasAllDecorators(this System.Reflection.PropertyInfo type, params Type[] decorators)
        {
            if (type == null)
            {
                throw new ArgumentException($"PropertyHasAllDecorators, type must be parsed");
            }

            if (decorators == null || decorators.Length == 0)
            {
                throw new ArgumentException($"PropertyHasAllDecorators, at least one decorator must be parsed");
            }

            foreach (var dec in decorators)
            {
                var attr = type.GetCustomAttributes(dec, false).FirstOrDefault();
                if (attr == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static Type PropertyFirstGenericTypeOf(this System.Reflection.PropertyInfo property)
        {
            if (!property.PropertyType.IsGenericType)
            {
                throw new Exception($"Property: { property } is not a generic type.");
            }

            if (property.PropertyType.GenericTypeArguments == null || property.PropertyType.GenericTypeArguments.Length == 0)
            {
                throw new Exception($"Property: {property} does not have any generic argument types");
            }
            return property.PropertyType.GenericTypeArguments.First(); 
        }

        public static string ToSdkTypeName(this Type type)
        {
            switch (type.FullName)
            {
                case "System.Int32": return "Microsoft.Crm.Sdk.CrmNumber, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "System.Decimal": return "Microsoft.Crm.Sdk.CrmDecimal, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "System.Double": return "Microsoft.Crm.Sdk.CrmFloat, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "System.DateTime": return "Microsoft.Crm.Sdk.CrmDateTime, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "Microsoft.Xrm.Sdk.Money": return "Microsoft.Crm.Sdk.CrmMoney, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "Microsoft.Xrm.Sdk.OptionSetValue": return "Microsoft.Crm.Sdk.Picklist, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "System.String": return "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                case "System.Boolean": return "Microsoft.Crm.Sdk.CrmBoolean, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                case "Microsoft.Xrm.Sdk.EntityReference": return "Microsoft.Crm.Sdk.Lookup, Microsoft.Crm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
            }

            throw new Exception($"Type { type.FullName } has not been mapped to a supported Crm SDK workflow type type");
         }
    }
}
