using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.kipon_multitest
{
    public class kipon_multitestPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Entities.kipon_multitest target, Kipon.Xrm.IRepository<Entities.kipon_multitest> repo)
        {
            var queryWithoutName = (from r in repo.GetQuery()
                                 where r.kipon_name == "ROOT"
                                 select r.kipon_multitestId).Single();

            var queryWithName = (from r in repo.GetQuery()
                                 where r.kipon_multitestId == queryWithoutName.Value
                                 select new
                                 {
                                     Id = r.kipon_multitestId.Value,
                                     Name = r.kipon_name
                                 }).Single();

            if (string.IsNullOrEmpty(queryWithName.Name))
            {
                throw new InvalidPluginExecutionException("name did not have a value");
            }

            target.kipon_name = $"{ queryWithoutName.Value.ToString() } did have a name: { queryWithName.Name }";

        }
    }
}
