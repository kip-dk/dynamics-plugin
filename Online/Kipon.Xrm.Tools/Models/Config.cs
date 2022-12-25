using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Tools.Models
{
    [DataContract]
    public class Config
    {
        public static readonly Config Instance;

        static Config()
        {
            var file = "config".CommandlineValue();
            if (!string.IsNullOrEmpty(file))
            {
                using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Open))
                {
                    Instance = InitializeFromStream(fs);
                }
            } 
        }

        [DataMember(Name = "connectionString")]
        public string ConnectionString { get; set; }

        private static Config InitializeFromStream(System.IO.Stream str)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Config));

            return (Config)ser.ReadObject(str);
        }
    }
}
