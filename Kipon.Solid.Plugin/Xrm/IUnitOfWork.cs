namespace Kipon.Xrm
{
    using Microsoft.Xrm.Sdk;
    using System;
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
