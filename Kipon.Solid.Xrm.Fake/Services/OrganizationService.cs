using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Kipon.Xrm.Fake.Services
{
    public class OrganizationService : Microsoft.Xrm.Sdk.IOrganizationService, Microsoft.Xrm.Sdk.IProxyTypesAssemblyProvider
    {
        private Repository.PluginExecutionFakeContext context;
        private Repository.IEntityShadow shadow;

        public Guid? UserId { get; private set; }
        public Assembly ProxyTypesAssembly { get; set; }

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
            return this.ToEarlyBoundEntity(nextResult);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var qr = new Repository.Query.QueryResolver();
            var r = qr.ExecuteQuery(query, this.shadow.AllEntities());
            var ebs = (from m in r.Entities select this.ToEarlyBoundEntity(m)).ToArray();

            r.Entities.Clear();
            r.Entities.AddRange(ebs);
            return r;
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
            if (request.GetType() == typeof(Microsoft.Xrm.Sdk.Messages.RetrieveMultipleRequest))
            {
                var req = (Microsoft.Xrm.Sdk.Messages.RetrieveMultipleRequest)request;
                var results = this.RetrieveMultiple(req.Query);

                var response = new Microsoft.Xrm.Sdk.Messages.RetrieveMultipleResponse()
                {
                    Results = new ParameterCollection
                    {
                        { "EntityCollection", results }
                    }
                };
                return response;
            }

            throw new NotImplementedException();
        }

        private Dictionary<string, Type> strongtypeEntities;

        private Entity ToEarlyBoundEntity(Entity source)
        {
            if (source == null)
            {
                return null;
            }

            if (this.ProxyTypesAssembly == null)
            {
                return source;
            }

            if (strongtypeEntities == null)
            {
                strongtypeEntities = new Dictionary<string, Type>();
                var entityClasses = this.ProxyTypesAssembly.GetTypes();
                foreach (var ent in entityClasses)
                {
                    var ca = (Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute)ent.GetCustomAttribute(typeof(Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute), false);
                    if (ca != null)
                    {
                        strongtypeEntities.Add(ca.LogicalName, ent);
                    }
                }
            }

            if (strongtypeEntities.ContainsKey(source.LogicalName))
            {
                var e = (Microsoft.Xrm.Sdk.Entity)Activator.CreateInstance(strongtypeEntities[source.LogicalName]);
                e.LogicalName = source.LogicalName;
                e.Id = source.Id;
                e.Attributes = source.Attributes;
                return e;
            }
            return source;
        }
    }
}
