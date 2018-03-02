using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Entities
{
    public class CrmUnitOfWork : IUnitOfWork, IDisposable
    {
        #region Members
        protected ContextService context;
        public IOrganizationService Service { get; private set; }
        private IOrganizationServiceFactory ServiceFactory { get; set; }
        #endregion

        #region Constructor

        public CrmUnitOfWork()
        {
            var serviceUri = new Uri(Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmServiceUrl);
            var orgServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(serviceUri);

            var credentials = Kipon.PluginRegistration.Security.Xrm.AuthenticationHelper.GetCredentials(orgServiceManagement, Kipon.PluginRegistration.Security.Configuration.ConfigurationSingleton.Section.XrmAuthenticationProviderType);
            var orgService = Kipon.PluginRegistration.Security.Xrm.AuthenticationHelper.GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, credentials);

            orgService.Timeout = new TimeSpan(0, 15, 0);

            orgService.EnableProxyTypes(typeof(ContextService).Assembly);

            Service = (IOrganizationService)orgService;


            this.context = new ContextService(Service);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        #region IUnitOfWork

        #region generic methods
        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        /// <summary>
        /// Executes a CRM request.
        /// </summary>
        /// <typeparam name="R">Type of the response.</typeparam>
        /// <param name="request">Request to execute.</param>
        public R ExecuteRequest<R>(Microsoft.Xrm.Sdk.OrganizationRequest request) where R : OrganizationResponse
        {
            return (R)this.context.Execute(request);
        }

        public OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request)
        {
            return this.context.Execute(request);
        }

        public Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request, Guid runAsSystemUserID)
        {
            var ser = this.ServiceFactory.CreateOrganizationService(runAsSystemUserID);
            return ser.Execute(request);
        }

        public Guid Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            return this.Service.Create(entity);
        }

        public void Update(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Service.Update(entity);
        }

        public void Delete(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Service.Delete(entity.LogicalName, entity.Id);
        }
        #endregion

        #region dynamic standard entities

        private IRepository<Solution> _solutions;
        public IRepository<Solution> Solutions
        {
            get
            {
                if (_solutions == null)
                {
                    _solutions = new CrmRepository<Solution>(this.context);
                }
                return _solutions;
            }
        }

        private IRepository<SolutionComponent> _solutioncomponents;
        public IRepository<SolutionComponent> SolutionComponents
        {
            get
            {
                if (_solutioncomponents == null)
                {
                    _solutioncomponents = new CrmRepository<SolutionComponent>(this.context);
                }
                return _solutioncomponents;
            }
        }

        private IRepository<PluginAssembly> _pluginAssemblies;
        public IRepository<PluginAssembly> PluginAssemblies
        {
            get
            {
                if (_pluginAssemblies == null)
                {
                    _pluginAssemblies = new CrmRepository<PluginAssembly>(this.context);
                }
                return _pluginAssemblies;
            }
        }

        private IRepository<SdkMessageProcessingStep> _sdkMessageProcessingSteps;
        public IRepository<SdkMessageProcessingStep> SdkMessageProcessingSteps
        {
            get
            {
                if (_sdkMessageProcessingSteps == null)
                {
                    _sdkMessageProcessingSteps = new CrmRepository<SdkMessageProcessingStep>(this.context);
                }
                return _sdkMessageProcessingSteps;
            }
        }

        private IRepository<SdkMessageProcessingStepImage> _sdkMessageProcessingStepImages;
        public IRepository<SdkMessageProcessingStepImage> SdkMessageProcessingStepImages
        {
            get
            {
                if (_sdkMessageProcessingStepImages == null)
                {
                    _sdkMessageProcessingStepImages = new CrmRepository<SdkMessageProcessingStepImage>(this.context);
                }
                return _sdkMessageProcessingStepImages;
            }
        }

        private IRepository<PluginType> _plugintypes;
        public IRepository<PluginType> PluginTypes
        {
            get
            {
                if (_plugintypes == null)
                {
                    _plugintypes = new CrmRepository<PluginType>(this.context);
                }
                return _plugintypes;
            }
        }

        private IRepository<SdkMessage> _sdkMessages;
        public IRepository<SdkMessage> SdkMessages
        {
            get
            {
                if (_sdkMessages == null)
                {
                    _sdkMessages = new CrmRepository<SdkMessage>(this.context);
                }
                return _sdkMessages;
            }
        }

        private IRepository<SdkMessageFilter> _sdkMessageFilters;
        public IRepository<SdkMessageFilter> SdkMessageFilters
        {
            get
            {
                if (_sdkMessageFilters == null)
                {
                    this._sdkMessageFilters = new CrmRepository<SdkMessageFilter>(this.context);
                }
                return _sdkMessageFilters;
            }
        }
        #endregion

        public void ClearChanges()
        {
            this.context.ClearChanges();
        }
        #endregion
    }
}
