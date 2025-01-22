
namespace Kipon.Xrm.Extensions.Json
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class JsonMethods
    {
        private static Dictionary<string, string>[] DIC_ARR = new Dictionary<string, string>[0];
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
            else
            {
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(str);
            }
        }

        public static Dictionary<string, string>[] ToDictionaryArray(this System.IO.Stream str)
        {
            return str.Deserialze<Dictionary<string, string>[]>();
        }
    }
}
