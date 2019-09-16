using Kipon.Dynamics.Plugin.DI;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Entities
{
    [Export(typeof(IUnitOfWork))]
    public partial class CrmUnitOfWork : IUnitOfWork, IDisposable
    {
        private ContextService context;


        private IOrganizationService _service;
        [Import]
        public IOrganizationService Service
        {
            get { return _service;  }
            set
            {
                this._service = value;
                this.context = new ContextService(_service);
            }
        }

        [Import]
        public IOrganizationServiceFactory ServiceFactory { get; set; }


        public CrmUnitOfWork()
        {
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }


        public void Dispose()
        {
            context.Dispose();
        }

        public Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request, Guid runAsSystemUserID)
        {
            var ser = this.ServiceFactory.CreateOrganizationService(runAsSystemUserID);
            return ser.Execute(request);
        }


        public R ExecuteRequest<R>(Microsoft.Xrm.Sdk.OrganizationRequest request) where R : OrganizationResponse
        {
            return (R)this.context.Execute(request);
        }

        public OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request)
        {
            return this.context.Execute(request);
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
        public void ClearChanges()
        {
            this.context.ClearChanges();
        }

        public void Detach(string logicalName, Guid? id)
        {
            if (this.context != null)
            {
                var candidates = (from c in this.context.GetAttachedEntities() where c.LogicalName == logicalName select c);
                if (id != null)
                {
                    candidates = (from c in candidates where c.Id == id.Value select c);
                }
                foreach (var r in candidates.ToArray())
                {
                    context.Detach(r);
                }
            }
        }
    }
}
