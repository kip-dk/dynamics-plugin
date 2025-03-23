
namespace Kipon.Xrm.Extensions.Json
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class JsonMethods
    {
        private static Dictionary<string, string>[] DIC_ARR = new Dictionary<string, string>[0];
        private static Dictionary<string, string> DIC_ARR_ROW = new Dictionary<string, string>();
        public static T Deserialze<T>(this System.IO.Stream str)
        {
            if (typeof(T) == DIC_ARR.GetType())
            {
                var setting = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(DIC_ARR.GetType(), setting);
                return (T)ser.ReadObject(str);
            }

            if (typeof(T) == DIC_ARR_ROW.GetType())
            {
                var setting = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(DIC_ARR_ROW.GetType(), setting);
                return (T)ser.ReadObject(str);
            }

            {
                var setting = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };

                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), setting);
                return (T)ser.ReadObject(str);
            }
        }

        public static Dictionary<string, string>[] ToDictionaryArray(this System.IO.Stream str)
        {
            return str.Deserialze<Dictionary<string, string>[]>();
        }
    }
}
