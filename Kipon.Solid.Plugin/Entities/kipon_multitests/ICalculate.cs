using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class kipon_multitest : kipon_multitest.ICalculate
    {
        public interface ICalculate : Ikipon_multitestTarget
        {
            bool? kipon_calculate { get; set; }

            int? kipon_number { set; }
        }
    }
}
