using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class kipon_datepoc : kipon_datepoc.IDateChanged
    {
        public interface IDateChanged : Ikipon_datepocMergedimage
        {
            [TargetFilter]
            DateTime? kipon_dateonly { get; }

            string kipon_name { get; }
        }
    }
}
