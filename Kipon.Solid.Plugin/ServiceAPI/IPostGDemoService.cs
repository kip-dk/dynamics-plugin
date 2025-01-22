using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.ServiceAPI
{
    public interface IPostGDemoService
    {
        Microsoft.Xrm.Sdk.Entity Get(string logicalname, Guid id);
        Microsoft.Xrm.Sdk.EntityCollection Query(Microsoft.Xrm.Sdk.Query.QueryExpression query,  params string[] quickfindfields);
    }
}
