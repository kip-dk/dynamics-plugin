using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Entities
{
    public partial class CrmUnitOfWork : IUnitOfWork, IDisposable
    {
        #region Members
        protected ContextService context;
        public IOrganizationService Service { get; private set; }
        private IOrganizationServiceFactory ServiceFactory { get; set; }
        #endregion

        #region Constructor

        public CrmUnitOfWork(IOrganizationService service, IOrganizationServiceFactory serviceFactory)
        {
            this.Service = service;
            this.ServiceFactory = serviceFactory;
            this.context = new ContextService(service);
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

        public void ClearChanges()
        {
            this.context.ClearChanges();
        }
        #endregion
    }
}
