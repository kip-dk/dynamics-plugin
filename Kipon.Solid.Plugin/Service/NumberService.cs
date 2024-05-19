using Kipon.Solid.Plugin.Entities;
using Kipon.Xrm;
using Kipon.Xrm.Attributes;
using Kipon.Xrm.Extensions.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Service
{
    public class NumberService : ServiceAPI.INumberService
    {
        private readonly IRepository<kipon_number> numbeRepo;

        public NumberService([Admin]Kipon.Xrm.IRepository<Entities.kipon_number> numbeRepo)
        {
            this.numbeRepo = numbeRepo;
        }

        public int Next(int serieNumber)
        {
            var id = serieNumber.ToGuid();
            var clean = new Entities.kipon_number { kipon_numberId = id };
            clean.kipon_lock = Guid.NewGuid().ToString();
            numbeRepo.Update(clean);

            return (from n in numbeRepo.GetQuery()
                    where n.kipon_numberId == id
                      && n.kipon_lock == clean.kipon_lock
                    select n.kipon_latest.Value).Single();
        }
    }
}
