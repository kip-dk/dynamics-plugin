
namespace Kipon.Xrm.Extensions.Sdk
{
    using Microsoft.Xrm.Sdk;

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

            if (!string.IsNullOrEmpty(refid.Name))
            {
                return refid.Name;
            }

            if (metas.TryGetValue(refid.LogicalName, out Meta m))
            {
                var result = this.organizationService.Retrieve(refid.LogicalName, refid.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(m.PrimaryAttributeName));
                refid.Name = result[m.PrimaryAttributeName] as string;
                return refid.Name;
            }
            return null;
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
