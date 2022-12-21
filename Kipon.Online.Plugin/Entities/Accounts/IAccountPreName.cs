using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Account.IAccountPreName
    {
        public interface IAccountPreName : IAccountPreimage
        {
            string Name { get; }
        }
    }
}
