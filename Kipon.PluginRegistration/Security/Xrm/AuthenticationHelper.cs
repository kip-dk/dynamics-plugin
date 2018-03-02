using Microsoft.Xrm.Sdk.Client;
using System;
using System.ServiceModel.Description;

namespace Kipon.PluginRegistration.Security.Xrm
{
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Obtain the AuthenticationCredentials based on AuthenticationProviderType.
        /// </summary>
        /// <param name="service">A service management object.</param>
        /// <param name="endpointType">An AuthenticationProviderType of the CRM environment.</param>
        /// <returns>Get filled credentials.</returns>
        public static AuthenticationCredentials GetCredentials<TService>(IServiceManagement<TService> service, AuthenticationProviderType endpointType)
        {
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();

            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential =
                        new System.Net.NetworkCredential(Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceUser,
                            Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServicePassword,
                            Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceDomain);
                    break;
                case AuthenticationProviderType.LiveId:
                    authCredentials.ClientCredentials.UserName.UserName =
                        Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceUser + "@" +
                        Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceDomain;
                    authCredentials.ClientCredentials.UserName.Password = Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServicePassword;
                    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                    authCredentials.SupportingCredentials.ClientCredentials = DeviceIdManager.LoadOrRegisterDevice();
                    break;

                // For Federated and OnlineFederated environments-
                default:
                    authCredentials.ClientCredentials.UserName.UserName =
                        Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceUser + "@" +
                        Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceDomain;
                    authCredentials.ClientCredentials.UserName.Password = Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServicePassword;
                    // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
                    // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  // Windows Kerberos

                    // The service is configured for User Id authentication, but the user might provide Microsoft
                    // account credentials. If so, the supporting credentials must contain the device credentials.
                    if (endpointType == AuthenticationProviderType.OnlineFederation)
                    {
                        IdentityProvider provider = service.GetIdentityProvider(authCredentials.ClientCredentials.UserName.UserName);
                        if (provider != null && provider.IdentityProviderType == IdentityProviderType.LiveId)
                        {
                            authCredentials.SupportingCredentials = new AuthenticationCredentials();
                            authCredentials.SupportingCredentials.ClientCredentials = DeviceIdManager.LoadOrRegisterDevice();
                        }
                    }

                    break;
            }

            return authCredentials;
        }

        public static TProxy GetProxy<TService, TProxy>(IServiceManagement<TService> serviceManagement, AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            var classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType !=
                AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(authCredentials);
                // Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
                // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }

    }
}
