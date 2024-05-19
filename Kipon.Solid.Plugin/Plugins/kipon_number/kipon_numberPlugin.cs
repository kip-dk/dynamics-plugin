using Kipon.Xrm.Extensions.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.kipon_number
{
    public class kipon_numberPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnValidateCreate(Entities.kipon_number target, IQueryable<Entities.kipon_number> query)
        {
            var count = (from q in query
                         select q.kipon_numberId.Value).ToArray().Count() + 1;

            target.kipon_numberId = count.ToGuid();

            if (target.kipon_latest == null)
            {
                target.kipon_latest = 0;
            }
        }

        public void OnPreUpdate(Entities.kipon_number.ILockChanged mergedimage)
        {
            mergedimage.Increment();
        }
    }
}
