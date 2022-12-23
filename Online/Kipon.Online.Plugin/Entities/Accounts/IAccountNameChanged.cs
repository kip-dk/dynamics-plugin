using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Account : Account.IAccountNameChanged
    {
        void Account.IAccountNameChanged.setDescription(string v)
        {
            this.Description = v;
        }

        public interface IAccountNameChanged : IAccountTarget
        {
            [Required]
            string Name { get; set; }

            void setDescription(string v);
        }
    }
}
