using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.kipon_multitest
{
    public class kipon_multitestPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Entities.kipon_multitest target, Kipon.Xrm.IRepository<Entities.kipon_multitest> repo)
        {
            var queryWithoutName = (from r in repo.GetQuery()
                                 where r.kipon_Name == "ROOT"
                                 select r.kipon_multitestId).SingleOrDefault();

            if (queryWithoutName != null)
            {
                var queryWithName = (from r in repo.GetQuery()
                                     where r.kipon_multitestId == queryWithoutName.Value
                                     select new
                                     {
                                         Id = r.kipon_multitestId.Value,
                                         Name = r.kipon_Name
                                     }).Single();

                if (string.IsNullOrEmpty(queryWithName.Name))
                {
                    throw new InvalidPluginExecutionException("name did not have a value");
                }

                target.kipon_Name = $"{queryWithoutName.Value.ToString()} did have a name: {queryWithName.Name}";
            }

        }
    }
}
