using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Account : Account.IAccountPostAccountNumber
    {
        public interface IAccountPostAccountNumber : IAccountPostimage
        {
            string AccountNumber { get; }
        }
    }
}
