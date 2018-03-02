using System.Configuration;

namespace Kipon.PluginRegistration.Security.Configuration
{
    public class KiponSecConfigurationSection : ConfigurationSection
    {
        #region Members

        static ConfigurationProperty propXrmServiceUrl;
        static ConfigurationProperty propXrmServiceUser;
        static ConfigurationProperty propXrmServiceDomain;
        static ConfigurationProperty propXrmServicePassword;
        static ConfigurationProperty propXrmAuthenticationProviderType;

        static ConfigurationPropertyCollection properties;

        #endregion

        #region Static constructor

        static KiponSecConfigurationSection()
        {
            propXrmServiceUrl = new ConfigurationProperty("xrmServiceUrl", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            propXrmServiceUser = new ConfigurationProperty("xrmServiceUser", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            propXrmServiceDomain = new ConfigurationProperty("xrmServiceDomain", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            propXrmServicePassword = new ConfigurationProperty("xrmServicePassword", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            propXrmAuthenticationProviderType = new ConfigurationProperty("xrmAuthenticationProviderType", typeof(Microsoft.Xrm.Sdk.Client.AuthenticationProviderType), Microsoft.Xrm.Sdk.Client.AuthenticationProviderType.ActiveDirectory, ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection();
            properties.Add(propXrmServiceUrl);
            properties.Add(propXrmServiceUser);
            properties.Add(propXrmServiceDomain);
            properties.Add(propXrmServicePassword);
            properties.Add(propXrmAuthenticationProviderType);
        }

        #endregion

        #region Properties

        [ConfigurationProperty("xrmServiceUrl", IsRequired = true)]
        public string XrmServiceUrl
        {
            get { return (string)base[propXrmServiceUrl]; }
            set { base[propXrmServiceUrl] = value; }
        }

        [ConfigurationProperty("xrmServiceUser", IsRequired = true)]
        public string XrmServiceUser
        {
            get { return (string)base[propXrmServiceUser]; }
            set { base[propXrmServiceUser] = value; }
        }

        [ConfigurationProperty("xrmServiceDomain", IsRequired = true)]
        public string XrmServiceDomain
        {
            get { return (string)base[propXrmServiceDomain]; }
            set { base[propXrmServiceDomain] = value; }
        }

        [ConfigurationProperty("xrmServicePassword", IsRequired = true)]
        public string XrmServicePassword
        {
            get { return (string)base[propXrmServicePassword]; }
            set { base[propXrmServicePassword] = value; }
        }

        [ConfigurationProperty("xrmAuthenticationProviderType", IsRequired = true)]
        public Microsoft.Xrm.Sdk.Client.AuthenticationProviderType XrmAuthenticationProviderType
        {
            get { return (Microsoft.Xrm.Sdk.Client.AuthenticationProviderType)base[propXrmAuthenticationProviderType]; }
            set { base[propXrmAuthenticationProviderType] = value; }
        }

        #endregion
    }
}
