using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Account.IAccountNameChanged
    {
        public interface IAccountNameChanged : IAccountTarget
        {
            [Required]
            string Name { get; set; }
        }
    }
}
