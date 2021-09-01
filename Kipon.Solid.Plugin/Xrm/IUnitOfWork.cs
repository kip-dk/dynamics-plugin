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
        void ClearContext();
        void Detach(string logicalname, params Guid[] ids);
        void Detach(Microsoft.Xrm.Sdk.EntityReference reference);
        void Detach(Microsoft.Xrm.Sdk.Entity entity);
        ServiceAPI.IEntityCache Cache { get; }
    }
}
