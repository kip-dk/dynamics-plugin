using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Entities
{
    [Export(typeof(IUnitOfWork))]
    public partial class CrmUnitOfWork : IUnitOfWork, IDisposable
    {
        #region Members
        protected ContextService context;
        public IOrganizationService Service { get; private set; }
        #endregion

        #region Constructor

        [ImportingConstructor]
        public CrmUnitOfWork()
        {
            this.Service = new Kipon.Xrm.Tools.XrmOrganization.OrganizationService();
            this.context = new ContextService(this.Service);
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

        public void ClearContext()
        {
            foreach (var ent in this.context.GetAttachedEntities().ToArray())
            {
                this.context.Detach(ent);
            }
        }

        public void Detach(string entitylogicalname, IEnumerable<Guid> ids)
        {
            var removes = (from en in this.context.GetAttachedEntities()
                          join id in ids on en.Id equals id
                          where en.LogicalName == entitylogicalname
                          select en).ToArray();
            if (removes != null && removes.Length > 0)
            {
                foreach (var re in removes)
                {
                    this.context.Detach(re);
                }
            }
        }

        public void Detach(string entitylogicalname, Guid id)
        {
            var remove = (from en in this.context.GetAttachedEntities()
                           where en.LogicalName == entitylogicalname
                             && en.Id == id
                           select en).SingleOrDefault();
            if (remove != null)
            {
                this.context.Detach(remove);
            }
        }

        public void Detach(string entitylogicalname)
        {
            var remove = (from en in this.context.GetAttachedEntities()
                          where en.LogicalName == entitylogicalname
                          select en).SingleOrDefault();

            if (remove != null)
            {
                this.context.Detach(remove);
            }
        }
        #endregion
    }
}
