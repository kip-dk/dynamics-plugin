using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Account : Account.ICreditLimitChanged
    {
        public interface ICreditLimitChanged : IAccountTarget
        {
            Microsoft.Xrm.Sdk.Money CreditLimit { get; set; }
        }
    }
}
