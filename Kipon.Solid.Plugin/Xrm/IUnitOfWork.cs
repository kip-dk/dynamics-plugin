using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm
{
    public interface IUnitOfWork
    {
        R ExecuteRequest<R>(OrganizationRequest request) where R : OrganizationResponse;
        OrganizationResponse Execute(OrganizationRequest request);
        System.Guid Create(Entity entity);
        void Update(Entity entity);
        void Delete(Entity entity);
        void SaveChanges();
        void ClearChanges();
        void Detach(string logicalname, Guid? id);
    }
}
