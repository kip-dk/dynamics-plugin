using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace Kipon.PluginRegistration.Security.Configuration
{
    public class ConfigurationSingleton
    {
        static KiponSecConfigurationSection section;
        static global::System.Configuration.Configuration config;

        private ConfigurationSingleton()
        {
        }

        public static KiponSecConfigurationSection Section
        {
            get
            {
                if (config == null)
                {
                    try
                    {
                        config = WebConfigurationManager.OpenWebConfiguration("~");
                    }
                    catch (Exception)
                    {
                    }

                    if (config == null)
                    {
                        config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    }

                    section = config.GetSection("kiponSec") as KiponSecConfigurationSection;
                }

                return section;
            }
        }
    }
}
