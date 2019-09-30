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
        private Repository.PluginEntityContext context;
        private Repository.IEntityShadow shadow;

        public Guid? UserId { get; private set; }

        public OrganizationService(Repository.PluginEntityContext context, Guid? userid)
        {
            this.UserId = userid;
            this.context = context;
            this.shadow = (Repository.IEntityShadow)context;
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            context.AddEntity(entity);
            return entity.Id;
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
