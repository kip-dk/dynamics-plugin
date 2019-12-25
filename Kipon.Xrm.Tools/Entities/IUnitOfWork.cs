using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Entities
{
    public partial interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits the applied changes.
        /// </summary>


        void SaveChanges();

        /// <summary>
        /// Executes a CRM request.
        /// </summary>
        /// <typeparam name="R">Type of the response.</typeparam>
        /// <param name="request">Request to execute.</param>
        R ExecuteRequest<R>(Microsoft.Xrm.Sdk.OrganizationRequest request) where R : Microsoft.Xrm.Sdk.OrganizationResponse;

        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request);

        System.Guid Create(Microsoft.Xrm.Sdk.Entity entity);
        void Update(Microsoft.Xrm.Sdk.Entity entity);
        void Delete(Microsoft.Xrm.Sdk.Entity entity);

        void ClearChanges();

    }
}
