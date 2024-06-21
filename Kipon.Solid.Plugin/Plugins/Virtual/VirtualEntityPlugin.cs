using Kipon.Xrm.Extensions.QueryExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Virtual
{
    public class VirtualEntityPlugin : Kipon.Xrm.VirtualEntityPlugin
    {
        public Microsoft.Xrm.Sdk.Entity OnRetrieve(Guid primaryentityid, string primaryentityname)
        {
            if (primaryentityname == Entities.kipon_vetest.EntityLogicalName)
            {
                return Entities.kipon_vetest.Testdata().Where(r => r.Id == primaryentityid).Single();
            }

            return new Microsoft.Xrm.Sdk.Entity { LogicalName = primaryentityname, Id = primaryentityid };
        }

        public Microsoft.Xrm.Sdk.EntityCollection OnRetrieveMultiple(string primaryentityname, Microsoft.Xrm.Sdk.Query.QueryExpression query, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace($"RM: { primaryentityname }");

            query.Trace(traceService);


            if (primaryentityname == Entities.kipon_vetest.EntityLogicalName)
            {
                return Entities.kipon_vetest.Testdata().Query(query, nameof(Entities.kipon_vetest.kipon_name));
            }

            var accountId = query.EntityReferenceIdEqualFilter("kipon_accountid");
            var quicksearch = query.QuickFindFilter();

            var result = new Microsoft.Xrm.Sdk.EntityCollection();
            for (var i = 0; i < 10; i++)
            {
                result.Entities.Add(new Microsoft.Xrm.Sdk.Entity { LogicalName = primaryentityname, Id = Guid.NewGuid() });
            }

            if (accountId != null)
            {
                var accountRef = new Microsoft.Xrm.Sdk.EntityReference { LogicalName = "account", Id = accountId.Value, Name = "account name" };
                foreach (var ent in result.Entities)
                {
                    ent["kipon_accountid"] = accountRef;
                }
            }

            if (!string.IsNullOrEmpty(quicksearch))
            {
                var ix = 1;
                foreach (var ent in result.Entities)
                {
                    ent["kipon_name"] = $"{ quicksearch}:{ ix }";
                    ix++;
                }
            }

            return result;
        }
    }
}
