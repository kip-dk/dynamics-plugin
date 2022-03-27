using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Account.IAccountNameAndSettersOnly
    {
        public interface IAccountNameAndSettersOnly : Kipon.Xrm.Target<Account>
        {
            string Name { get; }
            Microsoft.Xrm.Sdk.Money CreditLimit { set; }
        }
    }
}
