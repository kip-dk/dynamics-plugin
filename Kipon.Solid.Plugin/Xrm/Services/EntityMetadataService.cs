
namespace Kipon.Xrm.Services
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EntityMetadataService : ServiceAPI.IEntityMetadataService
    {
        private readonly IOrganizationService orgService;
        private static readonly Dictionary<string, MetaContainer> entities = new Dictionary<string, MetaContainer>();

        public EntityMetadataService(IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public EntityMetadata ForEntity(string logicalName)
        {
            if (entities.TryGetValue(logicalName, out MetaContainer m))
            {
                if (m.timeout < System.DateTime.Now)
                {
                    return m.meta;
                }
            }

            var req = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = logicalName
            };
            var res = (RetrieveEntityResponse)this.orgService.Execute(req);
            var con = new MetaContainer
            {
                meta = res.EntityMetadata,
                timeout = System.DateTime.Now.AddMinutes(15)
            };
            entities[logicalName] = con;
            return con.meta;
        }

        public string PrimaryKey(string logicalName)
        {
            var meta = this.ForEntity(logicalName);
            return meta.Attributes.Where(r => r.IsPrimaryId == true).Select(r => r.LogicalName).Single();

        }
        public string PrimaryName(string logicalName)
        {
            var meta = this.ForEntity(logicalName);
            return meta.Attributes.Where(r => r.IsPrimaryName == true).Select(r => r.LogicalName).Single();
        }

        public string[] CanCreateAttributeName(string logicalName)
        {
            var meta = this.ForEntity(logicalName);
            return meta.Attributes.Where(r => r.IsValidForCreate == true).Select(r => r.LogicalName).ToArray();
        }

        public string[] CanUpdateAttributeName(string logicalName)
        {
            var meta = this.ForEntity(logicalName);
            return meta.Attributes.Where(r => r.IsValidForUpdate == true).Select(r => r.LogicalName).ToArray();
        }

        public Entity CloneForCreate(Entity source, params string[] ommit)
        {
            var allOmmit = new List<string> { this.PrimaryKey(source.LogicalName), "createdon", "createdby", "modifiedon", "modifiedby" };
            if (ommit != null && ommit.Length > 0)
            {
                allOmmit.AddRange(ommit);
            }

            var allInclude = this.CanCreateAttributeName(source.LogicalName);

            var result = new Entity(source.LogicalName);
            foreach (var key in source.Attributes.Keys)
            {
                if (allInclude.Contains(key) && !ommit.Contains(key))
                {
                    result[key] = source[key];
                }
            }
            return result;
        }



        internal class MetaContainer
        {
            internal DateTime timeout { get; set; }
            internal EntityMetadata meta { get; set; }
        }
    }
}
