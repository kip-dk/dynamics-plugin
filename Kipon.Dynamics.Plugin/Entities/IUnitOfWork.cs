using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Entities
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits the applied changes.
        /// </summary>


        void SaveChanges();

        #region Dynamic standard entities
        /// <summary>
        /// Accounts.
        /// </summary>
        IRepository<Account> Accounts { get; }
        /// <summary>
        /// Contacts.
        /// </summary>
        IRepository<Contact> Contacts { get; }

        IRepository<Opportunity> Opportunities { get; }

        IRepository<Quote> Quotes { get; }

        #endregion

        /// <summary>
        /// Executes a CRM request.
        /// </summary>
        /// <typeparam name="R">Type of the response.</typeparam>
        /// <param name="request">Request to execute.</param>
        R ExecuteRequest<R>(Microsoft.Xrm.Sdk.OrganizationRequest request) where R : Microsoft.Xrm.Sdk.OrganizationResponse;

        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request);
        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request, Guid runAsSystemUserID);

        System.Guid Create(Microsoft.Xrm.Sdk.Entity entity);
        void Update(Microsoft.Xrm.Sdk.Entity entity);
        void Delete(Microsoft.Xrm.Sdk.Entity entity);

        void ClearChanges();

    }
}
