using Kipon.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Contact
{
    public class MergeContactPlugin : BasePlugin
    {
        public void OnPreMerge(Entities.ContactReference target, Guid SubordinateId)
        {
            Kipon.Xrm.Tracer.Trace($"{target.Value.LogicalName}.{target.Value.Id} => {SubordinateId}");
        }
    }
}
