using Kipon.Xrm.Extensions.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Service
{
    public class PostGDemoService : Kipon.Xrm.ServiceAPI.AbstractPostGRestService, ServiceAPI.IPostGDemoService
    {
        public PostGDemoService(Microsoft.Xrm.Sdk.IOrganizationService orgService): base(orgService)
        {
        }

        protected override string BaseURL => "https://data.geus.dk/gw-jupiter";

        protected override string FromId(Guid id)
        {
            return id.ToString().Split('-').First().ToInt().ToString();
        }

        protected override Guid ToId(string value)
        {
            return value.ToInt().ToGuid();
        }
    }
}
