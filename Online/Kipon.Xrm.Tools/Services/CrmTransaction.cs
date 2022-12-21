using Kipon.Xrm.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    public class CrmTransaction
    {
        private Microsoft.Xrm.Sdk.Messages.ExecuteTransactionRequest trans;

        public CrmTransaction()
        {
            this.trans = new Microsoft.Xrm.Sdk.Messages.ExecuteTransactionRequest
            {
                Requests = new Microsoft.Xrm.Sdk.OrganizationRequestCollection(),
                ReturnResponses = false
            };
        }

        public void Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.trans.Requests.Add(new Microsoft.Xrm.Sdk.Messages.CreateRequest { Target = entity });
        }

        public void Update(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.trans.Requests.Add(new Microsoft.Xrm.Sdk.Messages.UpdateRequest { Target = entity });
        }

        public void Delete(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.trans.Requests.Add(new Microsoft.Xrm.Sdk.Messages.DeleteRequest { Target = entity.ToEntityReference() });
        }

        public void Execute(Microsoft.Xrm.Sdk.OrganizationRequest request)
        {
            this.trans.Requests.Add(request);
        }

        public void Commit(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            if (trans.Requests != null && trans.Requests.Count > 0)
            {
                orgService.Execute(trans);
                this.trans = new Microsoft.Xrm.Sdk.Messages.ExecuteTransactionRequest
                {
                    Requests = new Microsoft.Xrm.Sdk.OrganizationRequestCollection(),
                    ReturnResponses = false
                };
            }
        }

        public void Commit(Entities.IUnitOfWork uow)
        {
            if (trans.Requests != null && trans.Requests.Count > 0)
            {
                uow.Execute(trans);
                this.trans = new Microsoft.Xrm.Sdk.Messages.ExecuteTransactionRequest
                {
                    Requests = new Microsoft.Xrm.Sdk.OrganizationRequestCollection(),
                    ReturnResponses = false
                };
            }
        }

    }
}
