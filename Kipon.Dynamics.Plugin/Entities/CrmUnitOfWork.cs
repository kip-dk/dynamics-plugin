using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Entities
{
    public class CrmUnitOfWork : IUnitOfWork, IDisposable
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

        #region dynamic standard entities
        private IRepository<Account> _account;
        public IRepository<Account> Accounts
        {
            get
            {
                if (_account == null)
                {
                    _account = new CrmRepository<Account>(this.context);
                }

                return _account;
            }
        }

        private IRepository<Contact> _contact;
        public IRepository<Contact> Contacts
        {
            get
            {
                if (_contact == null)
                {
                    _contact = new CrmRepository<Contact>(this.context);
                }

                return _contact;
            }
        }

        private IRepository<Opportunity> _opportunities;
        public IRepository<Opportunity> Opportunities
        {
            get
            {
                if (_opportunities == null)
                {
                    _opportunities = new CrmRepository<Opportunity>(this.context);
                }

                return _opportunities;
            }
        }

        private IRepository<Quote> _quotes;
        public IRepository<Quote> Quotes
        {
            get
            {
                if (_quotes == null)
                {
                    _quotes = new CrmRepository<Quote>(this.context);
                }

                return _quotes;
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
