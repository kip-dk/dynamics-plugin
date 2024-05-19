using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class kipon_number : kipon_number.ILockChanged
    {
        void ILockChanged.Increment()
        {
            this.kipon_latest = (this.kipon_latest ?? 0) + 1;
        }

        public interface ILockChanged : Ikipon_numberMergedimage
        {
            [TargetFilter]
            string kipon_lock { get; } 

            int? kipon_latest { get; set; }

            void Increment();
        }
    }
}
