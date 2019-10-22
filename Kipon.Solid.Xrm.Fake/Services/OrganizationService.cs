using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Kipon.Xrm.Fake.Services
{
    public class OrganizationService : Microsoft.Xrm.Sdk.IOrganizationService
    {
        private Repository.PluginExecutionFakeContext context;
        private Repository.IEntityShadow shadow;

        public Guid? UserId { get; private set; }

        public OrganizationService(Repository.PluginExecutionFakeContext context, Guid? userid)
        {
            this.UserId = userid;
            this.context = context;
            this.shadow = (Repository.IEntityShadow)context;
        }

        #region CRUD
        public Guid Create(Entity entity)
        {
            shadow.Create(entity);
            return entity.Id;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var result = this.shadow.Get(entityName, id);
            if (columnSet.AllColumns)
            {
                return result;
            }
            var nextResult = new Microsoft.Xrm.Sdk.Entity { LogicalName = entityName, Id = id };
            foreach (var col in columnSet.Columns)
            {
                nextResult[col] = result[col];
            }
            return nextResult;
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var qr = new Repository.Query.QueryResolver();
            return qr.ExecuteQuery(query, this.shadow.AllEntities());
        }

        public void Update(Entity entity)
        {
            shadow.Update(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            shadow.Delete(entityName, id);
        }
        #endregion

        #region associate
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var dis = new Microsoft.Xrm.Sdk.Messages.AssociateRequest
            {
                Relationship = relationship,
                Target = new EntityReference(entityName, entityId),
                RelatedEntities = relatedEntities
            };
            this.Execute(dis);
        }


        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var dis = new Microsoft.Xrm.Sdk.Messages.DisassociateRequest
            {
                Relationship = relationship,
                Target = new EntityReference(entityName, entityId),
                RelatedEntities = relatedEntities
            };
            this.Execute(dis);
        }
        #endregion

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
