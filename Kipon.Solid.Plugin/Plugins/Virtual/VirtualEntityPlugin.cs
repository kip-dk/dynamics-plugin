using Kipon.Xrm.Attributes;
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
        #region kipon_vetest
        [LogicalName(Entities.kipon_vetest.EntityLogicalName)]
        public Microsoft.Xrm.Sdk.Entity OnRetrieve(Guid primaryentityid, string primaryentityname)
        {
            return Entities.kipon_vetest.Testdata().Where(r => r.Id == primaryentityid).Single();
        }

        [LogicalName(Entities.kipon_vetest.EntityLogicalName)]
        public Microsoft.Xrm.Sdk.EntityCollection OnRetrieveMultiple(string primaryentityname, Microsoft.Xrm.Sdk.Query.QueryExpression query, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            return Entities.kipon_vetest.Testdata().Query(query, nameof(Entities.kipon_vetest.kipon_name));

        }
        #endregion

        #region unknown
        [LogicalName("unknown")]
        public Microsoft.Xrm.Sdk.Entity OnRetrieve(Guid primaryentityid, string primaryentityname, ServiceAPI.IAccountService accountService)
        {
            return new Microsoft.Xrm.Sdk.Entity { LogicalName = primaryentityname, Id = primaryentityid };
        }

        [LogicalName("unknown")]
        public Microsoft.Xrm.Sdk.EntityCollection OnRetrieveMultiple(string primaryentityname, Microsoft.Xrm.Sdk.Query.QueryExpression query, ServiceAPI.IAccountService accountService)
        {
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
                    ent["kipon_name"] = $"{quicksearch}:{ix}";
                    ix++;
                }
            }
            return result;
        }
        #endregion

        #region postgtest
        [LogicalName("kipon_postg")]
        public Microsoft.Xrm.Sdk.Entity OnRetrieve(Guid primaryentityid, string primaryentityname, ServiceAPI.IPostGDemoService postgService)
        {
            return postgService.Get(primaryentityname, primaryentityid);
        }

        [LogicalName("kipon_postg")]
        public Microsoft.Xrm.Sdk.EntityCollection OnRetrieveMultiple(string primaryentityname, Microsoft.Xrm.Sdk.Query.QueryExpression query, ServiceAPI.IPostGDemoService postgService)
        {
            return postgService.Query(query, "kipon_location", "kipon_name");
        }
        #endregion
    }
}