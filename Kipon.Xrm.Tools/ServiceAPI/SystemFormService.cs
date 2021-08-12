using Kipon.Xrm.Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    [Export(typeof(ServiceAPI.ISystemFormService))]
    public class SystemFormService : ServiceAPI.ISystemFormService
    {
        private readonly Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public SystemFormService(Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public SystemForm GetForm(string entityLogicalName, string formName)
        {
            var forms = (from f in uow.SystemForms.GetQuery()
                         where f.ObjectTypeCode == entityLogicalName
                           && f.Name == formName
                           && f.Type.Value == 2
                           && f.FormActivationState.Value != 0
                         select new SystemForm
                         {
                         }).ToArray();
            if (forms.Length == 0)
            {
                throw new Exception($"Form {formName} on entity {entityLogicalName} was not found.");
            }

            if (forms.Length > 1)
            {
                throw new Exception($"More than one form with name {formName} was found on entity {entityLogicalName}.");
            }

            var req = new Microsoft.Xrm.Sdk.Messages.RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.All
            };

            var resp = uow.ExecuteRequest<Microsoft.Xrm.Sdk.Messages.RetrieveEntityResponse>(req);

            forms[0].Parse(resp.EntityMetadata);
            return forms[0];
        }
    }
}
