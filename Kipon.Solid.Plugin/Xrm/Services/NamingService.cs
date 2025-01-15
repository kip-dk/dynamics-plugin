
namespace Kipon.Xrm.Extensions.Sdk
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Linq;
    using System.Text;

    public partial class NamingService : ServiceAPI.INamingService
    {
        private readonly IOrganizationService organizationService;
        private static System.Collections.Generic.Dictionary<string, Meta> metas = new System.Collections.Generic.Dictionary<string, Meta>();

        public NamingService([Attributes.Admin]IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        public string NameOf(EntityReference refid)
        {
            if (refid == null)
            {
                return null;
            }

            if (refid.Name != null)
            {
                return refid.Name;
            }

            if (metas.TryGetValue(refid.LogicalName, out Meta m))
            {
                var result = this.organizationService.Retrieve(refid.LogicalName, refid.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(m.PrimaryAttributeName));
                if (result != null && result.Attributes != null && result.Attributes.ContainsKey(m.PrimaryAttributeName))
                {
                    refid.Name = result[m.PrimaryAttributeName] as string;
                }

                if (refid.Name == null)
                {
                    refid.Name = string.Empty;
                }

                return refid.Name;
            }
            return null;
        }

        public Microsoft.Xrm.Sdk.EntityReference[] NamesOf(string entityLogicalName, params Guid[] ids)
        {
            if (string.IsNullOrEmpty(entityLogicalName))
            {
                return null;
            }

            if (ids == null || ids.Length == 0)
            {
                return null;
            }

            if (metas.TryGetValue(entityLogicalName, out Meta meta))
            {
                var qe = new Microsoft.Xrm.Sdk.Query.QueryExpression(entityLogicalName);
                qe.ColumnSet.AddColumn(meta.PrimaryAttributeId);
                qe.ColumnSet.AddColumn(meta.PrimaryAttributeName);
                qe.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression(meta.PrimaryAttributeId, Microsoft.Xrm.Sdk.Query.ConditionOperator.In, ids));

                var result = this.organizationService.RetrieveMultiple(qe);

                return (from r in result.Entities
                        select new Microsoft.Xrm.Sdk.EntityReference
                        {
                            Id = (Guid)r[meta.PrimaryAttributeId],
                            LogicalName = entityLogicalName,
                            Name = (string)r[meta.PrimaryAttributeName]
                        }).ToArray();
            }
            return null;
        }

        public string Concat(params Microsoft.Xrm.Sdk.EntityReference[] refs)
        {
            return this.Concat(", ", refs);
        }
        public string Concat(string sep, params Microsoft.Xrm.Sdk.EntityReference[] refs)
        {
            if (refs == null || refs.Length == 0)
            {
                return null;
            }

            var comma = string.Empty;
            var sb = new StringBuilder();

            foreach (var re in refs)
            {
                if (re != null)
                {
                    var next = this.NameOf(re);
                    if (!string.IsNullOrEmpty(next))
                    {
                        sb.Append($"{comma}{next}");
                        comma = sep;
                    }
                }
            }
            return sb.ToString();
        }


        public string PrimaryAttributeId(string entitylogicalname)
        {
            if (metas.TryGetValue(entitylogicalname, out Meta m))
            {
                return m.PrimaryAttributeId;
            }
            return null;
        }
        public string PrimaryAttributeName(string entitylogicalname)
        {
            if (metas.TryGetValue(entitylogicalname, out Meta m))
            {
                return m.PrimaryAttributeName;
            }
            return null;
        }

        private static void Add(string entitylogicalname, string entityattributeid, string entityattributename)
        {
            metas.Add(entitylogicalname, new Meta { PrimaryAttributeId = entityattributeid, PrimaryAttributeName = entityattributename });
        }

        private class Meta
        {
            internal string PrimaryAttributeId;
            internal string PrimaryAttributeName;
        }
    }
}
